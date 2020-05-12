using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffMeshSlidingBoard : BaseUniqueOffMeshLink
{
	enum State
	{
		FirstJump,
		Sliding,
		SecondJump,
	}
	/// <summary>ジャンプ目標地点 (world)</summary>
	Vector3 m_worldFirstJumpTarget { get { return pointTransform.LocalToWorldPosition(m_firstJumpTarget); } }
	/// <summary>二回目ジャンプ開始地点 (world)</summary>
	Vector3 m_worldSecondJumpStartPoint { get { return pointTransform.LocalToWorldPosition(m_secondJumpStartPoint); } }

	/// <summary>This ground flag</summary>Jump for specify time
	[SerializeField, Tooltip("滑り時の勾配検知BoxCast情報")]
	AdvanceBoxCastInfos m_slidingGradientDetectionInfos = default;

	/// <summary>ジャンプ目標地点 (local)</summary>
	[SerializeField, Space, Tooltip("ジャンプ目標地点(local)")]
	Vector3 m_firstJumpTarget = Vector3.zero;

	/// <summary>最初のJumpにかかる時間</summary>
	[SerializeField, Tooltip("最初のJumpにかかる時間")]
	float m_firstJumpSeconds = 1.5f;
	/// <summary>Jump中の回転速度時間</summary>
	[SerializeField, Tooltip("Jump中の回転速度時間")]
	float m_rotationSpeed = 3.0f;

	[SerializeField, Space, Tooltip("滑り初期実行時速度 (AddForce: Impluse)")]
	float m_startSlidingSpeed = 500.0f;
	[SerializeField, Tooltip("滑り時速度 (AddForce: Force)")]
	float m_slidingSpeed = 100.0f;

	[SerializeField, Space, Tooltip("二回目ジャンプ開始地点 (local)")]
	Vector3 m_secondJumpStartPoint = Vector3.zero;
	/// <summary>二回目ジャンプ開始地点の当たり判定->半径</summary>
	[SerializeField, Tooltip("二回目ジャンプ開始地点の当たり判定->半径")]
	float m_secondJumpStartPointRadius = 1.5f;
	/// <summary>二回目Jumpにかかる時間</summary>
	[SerializeField, Tooltip("二回目Jumpにかかる時間")]
	float m_secondJumpSeconds = 1.5f;

#if UNITY_EDITOR
	[Header("Debug Only"), SerializeField, Tooltip("Draw gizmos?")]
	bool m_dIsDrawGizmos = true;
	[SerializeField, Tooltip("Player name")]
	string m_dDrawGizmosPlayerObjectName = "Player";

	Transform m_dPlayerTransform = null;
#endif

	/// <summary>滑り時にisTrigger = falseにするCollider</summary>
	Collider[] m_slidingDisableTriggerCollisions = null;
	/// <summary>Timer</summary>
	Timer m_timer = new Timer();
	/// <summary>Sliding direction</summary>
	Vector3 m_slidingDirection = Vector3.zero;
	/// <summary>回転</summary>
	Quaternion lookRotation = Quaternion.identity;
	/// <summary>RaycastHit</summary>
	RaycastHit m_raycastHit = default;
	/// <summary>State</summary>
	State m_state = State.FirstJump;
	/// <summary>This collision triggers</summary>
	bool[] m_isCollisionTriggers = null;
	/// <summary>Is change collision triggers</summary>
	bool m_isChangeCollisionTriggers = false;

	protected override void StartOffMeshLink()
	{
		if (m_slidingDisableTriggerCollisions == null)
		{
			m_slidingDisableTriggerCollisions = GetComponentsInChildren<Collider>();
			m_isCollisionTriggers = new bool[m_slidingDisableTriggerCollisions.Length];
		}

		//IsTrigger取得, 一時的に全てtrueに
		for (int i = 0, length = m_slidingDisableTriggerCollisions.Length; i < length; ++i)
		{
			m_isCollisionTriggers[i] = m_slidingDisableTriggerCollisions[i].isTrigger;
			m_slidingDisableTriggerCollisions[i].isTrigger = true;
		}

		//RigitBody動作へ
		if (agentRigidBody.isKinematic)
			SwapEnableRigidAndAgent();

		//初期化
		m_state = State.FirstJump;
		m_isChangeCollisionTriggers = false;
		Vector3 worldFirstJumpPoint = m_worldFirstJumpTarget;
		Vector3 worldSecondJumpPoint = m_worldSecondJumpStartPoint;
		m_slidingDirection = new Vector3(worldSecondJumpPoint.x - worldFirstJumpPoint.x, 
			0.0f, worldSecondJumpPoint.z - worldFirstJumpPoint.z).normalized;

		//向くべき回転を設定
		var e = new Vector3(endPoint.x - startPoint.x, 0.0f, endPoint.z - startPoint.z).normalized;
		lookRotation = Quaternion.LookRotation(
			new Vector3(endPoint.x - startPoint.x, 0.0f, endPoint.z - startPoint.z).normalized);

		//ジャンプ実行
		Vector3 moveTarget = m_worldFirstJumpTarget;
		JumpForSpecifyTime.JumpExecution(agentRigidBody, agentTransform.position, moveTarget, m_firstJumpSeconds);
		//タイマースタート
		m_timer.Start();
	}
	protected override bool FixedUpdateOffMeshLink()
	{
		if (m_state == State.Sliding)
		{
			//終了？
			if ((m_worldSecondJumpStartPoint - agentTransform.position).sqrMagnitude 
				< m_secondJumpStartPointRadius * m_secondJumpStartPointRadius)
			{
				m_state = State.SecondJump;
				agentRigidBody.velocity = Vector3.zero;

				//向くべき回転を設定
				Vector3 position = agentTransform.position;
				//lookRotation = Quaternion.LookRotation(
				//	new Vector3(moveTargetPoint.x - position.x, 0.0f, moveTargetPoint.z - position.z).normalized);

				//ジャンプ実行
				JumpForSpecifyTime.JumpExecution(agentRigidBody, position, moveTargetPoint, m_secondJumpSeconds);
				//タイマースタート
				m_timer.Start();
			}
			//AddForce
			else if (m_slidingGradientDetectionInfos.BoxCast(agentTransform, Vector3.down, out m_raycastHit))
			{
				agentRigidBody.AddForce(
					Vector3.ProjectOnPlane(m_slidingDirection, m_raycastHit.normal) * m_slidingSpeed);
			}
		}
		return true;
	}

	protected override bool UpdateOffMeshLink()
	{
		switch (m_state)
		{
			case State.FirstJump:
				{
					//回転
					agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation,
						lookRotation, m_rotationSpeed * Time.deltaTime);

					//2/1経過でコリジョン.isTrigger無効化
					if (!m_isChangeCollisionTriggers && m_timer.elapasedTime >= m_firstJumpSeconds * 0.5f)
					{
						m_isChangeCollisionTriggers = true;
						for (int i = 0, length = m_slidingDisableTriggerCollisions.Length; i < length; ++i)
							m_slidingDisableTriggerCollisions[i].isTrigger = false;
					}
					//一定時間経過で終了
					else if (m_timer.elapasedTime >= m_firstJumpSeconds)
					{
						m_isChangeCollisionTriggers = false;
						m_state = State.Sliding;

						if (m_slidingGradientDetectionInfos.BoxCast(agentTransform, Vector3.down, out m_raycastHit))
						{
							agentRigidBody.AddForce(
								Vector3.ProjectOnPlane(m_slidingDirection, m_raycastHit.normal) * m_startSlidingSpeed, ForceMode.Impulse);
						}
					}
					return true;
				}
			case State.SecondJump:
				{   
					//回転
					agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation,
						lookRotation, m_rotationSpeed * Time.deltaTime);

					//2/1経過でコリジョン.isTriggerを戻す
					if (!m_isChangeCollisionTriggers && m_timer.elapasedTime >= m_firstJumpSeconds * 0.5f)
					{
						m_isChangeCollisionTriggers = true;
						for (int i = 0, length = m_slidingDisableTriggerCollisions.Length; i < length; ++i)
							m_slidingDisableTriggerCollisions[i].isTrigger = m_isCollisionTriggers[i];
					}

					//一定時間経過で終了
					return m_timer.elapasedTime < m_secondJumpSeconds;
				}
			default:
				return true;
		}
	}


	//Debug only
#if UNITY_EDITOR
	static readonly Color m_cdPurple = new Color(1.0f, 0.0f, 1.0f);

	void OnDrawGizmos()
	{
		if (!m_dIsDrawGizmos) return;

		Transform useTransform = pointTransform != null ? pointTransform : transform;
		Transform playerTransform = agentTransform != null ? agentTransform : m_dPlayerTransform;		
		Vector3 right = transform.right, up = transform.up, forward = transform.forward;

		if (playerTransform == null)
		{
			var obj = GameObject.Find(m_dDrawGizmosPlayerObjectName);
			if (obj != null)
				playerTransform = m_dPlayerTransform = obj.transform;
			else
				playerTransform = useTransform;
		}

		Vector3 target = useTransform.LocalToWorldPosition(m_firstJumpTarget);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(target, right);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(target, up);
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(target, forward);


		target = useTransform.LocalToWorldPosition(m_secondJumpStartPoint);
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(target, right);
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(target, up);
		Gizmos.color = m_cdPurple;
		Gizmos.DrawRay(target, forward);
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(target, m_secondJumpStartPointRadius);

		if (m_slidingGradientDetectionInfos.BoxCast(playerTransform, Vector3.down, out m_raycastHit))
			m_slidingGradientDetectionInfos.DOnDrawGizmos(playerTransform, Color.red, Vector3.down, m_raycastHit.distance);
		else
			m_slidingGradientDetectionInfos.DOnDrawGizmos(playerTransform, Color.white, Vector3.down);
	}
#endif
}
