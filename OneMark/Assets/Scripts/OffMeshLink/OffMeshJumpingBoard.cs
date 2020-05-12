using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffMeshJumpingBoard : BaseUniqueOffMeshLink
{
	enum State
	{
		FirstJump,
		SecondJumpWaitingStart,
		SecondJumpWaiting,
		SecondJump
	}
	/// <summary>中継目標地点 (world)</summary>
	Vector3 m_worldRelayPoint { get { return pointTransform.LocalToWorldPosition(m_relayPoint); } }

	/// <summary>中継目標地点 (local)</summary>
	[SerializeField, Tooltip("中継目標地点(local)")]
	Vector3 m_relayPoint = Vector3.zero;
	/// <summary>ため時下降Y座標 (transform.y - this)</summary>
	[SerializeField, Tooltip("ため時下降Y座標 (transform.y - this)")]
	float m_descentHeight = 0.3f;
	/// <summary>ため時下降加速度</summary>
	[SerializeField, Tooltip("ため時下降加速度")]
	float m_descentAccelerationSeconds = 0.1f;

	/// <summary>Jumpにかかる時間</summary>
	[SerializeField, Space, Tooltip("一回目Jumpにかかる時間")]
	float m_firstJumpSeconds = 1.5f;
	/// <summary>Jumpにかかる時間</summary>
	[SerializeField, Tooltip("二回目Jumpにかかる時間")]
	float m_secondJumpSeconds = 1.5f;
	/// <summary>Jump中の回転速度時間</summary>
	[SerializeField, Tooltip("Jump中の回転速度時間")]
	float m_rotationSpeed = 3.0f;

#if UNITY_EDITOR
	[Header("Debug Only"), SerializeField, Tooltip("Draw gizmos?")]
	bool m_dIsDrawGizmos = true;
#endif

	/// <summary>Timer</summary>
	Timer m_timer = new Timer();
	/// <summary>Agent Transform.position</summary>
	Vector3 m_position = Vector3.zero;
	/// <summary>回転</summary>
	Quaternion lookRotation = Quaternion.identity;
	/// <summary>State</summary>
	State m_state = State.FirstJump;
	/// <summary>ため時減少速度</summary>
	float m_decreasingSpeed = 0.0f;

	protected override void StartOffMeshLink()
	{
		//RigitBody動作へ
		if (agentRigidBody.isKinematic)
			SwapEnableRigidAndAgent();

		//初期化
		m_state = State.FirstJump;
		m_decreasingSpeed = 0.0f;
		m_position = agentTransform.position;

		//向くべき回転を設定
		Vector3 moveTarget = m_worldRelayPoint;
		lookRotation = Quaternion.LookRotation(
			new Vector3(moveTarget.x - startPoint.x, 0.0f, moveTarget.z - startPoint.z).normalized);

		//ジャンプ実行
		JumpForSpecifyTime.JumpExecution(agentRigidBody, agentTransform.position, moveTarget, m_firstJumpSeconds);

		//タイマースタート
		m_timer.Start();
	}

	protected override bool FixedUpdateOffMeshLink()
	{
		if (m_state == State.SecondJumpWaitingStart)
		{
			//停止
			agentRigidBody.velocity = Vector3.zero;
			agentRigidBody.useGravity = false;
			agentRigidBody.isKinematic = true;
			//Stateを進める
			m_state = State.SecondJumpWaiting;
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

					//一定時間経過で終了
					if (m_timer.elapasedTime >= m_firstJumpSeconds)
					{
						m_state = State.SecondJumpWaitingStart;
						agentRigidBody.velocity = Vector3.zero;
						agentRigidBody.useGravity = false;
						agentRigidBody.isKinematic = true;
					}
					return true;
				}
			case State.SecondJumpWaiting:
				{
					//加速
					m_decreasingSpeed += m_descentAccelerationSeconds * Time.fixedDeltaTime;
					//下降させる
					m_position = agentTransform.position;
					m_position.y -= m_decreasingSpeed * Time.fixedDeltaTime;

					float moveTarget = m_worldRelayPoint.y - m_descentHeight;
					//下降中
					if (m_position.y > moveTarget)
						agentTransform.position = m_position;
					//下降終了
					else
					{
						//設定
						m_position.y = moveTarget;
						agentTransform.position = m_position;
						//停止取り消し
						agentRigidBody.velocity = Vector3.zero;
						agentRigidBody.useGravity = true;
						agentRigidBody.isKinematic = false;
						//Stateを進める
						m_state = State.SecondJump;

						//向くべき回転を設定
						lookRotation = Quaternion.LookRotation(
							new Vector3(endPoint.x - startPoint.x, 0.0f, endPoint.z - startPoint.z).normalized);
						
						//ジャンプ実行
						JumpForSpecifyTime.JumpExecution(agentRigidBody, agentTransform.position, moveTargetPoint, m_secondJumpSeconds);
						//タイマースタート
						m_timer.Start();
					}

					return true;
				}
			case State.SecondJump:
				{
					//回転
					agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation,
						lookRotation, m_rotationSpeed * Time.deltaTime);

					//一定時間経過で終了
					return m_timer.elapasedTime < m_secondJumpSeconds;
				}
		}
		return true;
	}

	//Debug only
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (!m_dIsDrawGizmos) return;

		Transform useTransform = pointTransform != null ? pointTransform : transform;

		Vector3 target = useTransform.LocalToWorldPosition(m_relayPoint);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(target, useTransform.right);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(target, useTransform.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(target, useTransform.forward);
	}
#endif
}
