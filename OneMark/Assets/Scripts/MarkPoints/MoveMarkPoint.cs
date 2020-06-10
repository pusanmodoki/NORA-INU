using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 更新を行わないデフォルトクラスとなるDefaultMarkPoint 
/// </summary>
public class MoveMarkPoint : BaseMarkPoint
{
	enum State
	{
		MoveStart,
		Move,
		Wait,
	}

	static readonly float m_cRemainingCheckDistance = 0.1f * 0.1f;
	static readonly int m_cMoleAnimationEscapeMoveID = Animator.StringToHash("EscapeMove");
	static readonly int m_cMoleAnimationForceAppearID = Animator.StringToHash("ForceAppear");
	static readonly int m_cMoleAnimationAppearID = Animator.StringToHash("Appear");
	static readonly int m_cMoleAnimationStartMoveID = Animator.StringToHash("StartMove");

	[SerializeField, Space]
	NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	Transform m_mole = null;
	[SerializeField]
	Animator m_moleAnimator = null;
	[SerializeField]
	float m_waitSeconds = 1.0f;
	[SerializeField]
	Transform m_particleAdministratorObject = null;
	[SerializeField]
	ParticleSystem[] m_particleSystems = null;
	[SerializeField]
	Vector3[] m_targetPoints = null;

	ParticleSystem.MainModule m_mainModule = default;
	Quaternion[] m_firstRotations= null;
	Quaternion m_toNextRotation = Quaternion.identity;
	Vector3 m_toNextRotationEuler = Vector3.zero;
	Vector3 m_returnDestination = Vector3.zero;
	Vector2Int m_moveIndexInfo = new Vector2Int(0, 1);
	Timer m_timer = new Timer();
	State m_state = State.MoveStart;
	float m_moleDistance = 0.0f;
	float m_navMeshSpeed = 0.0f;
	bool m_isWaitMarking = false;
	bool m_isWaitMarkingEnd = false;


	public void AnimationEscapeCallback()
	{
		if (m_state == State.MoveStart && !m_isWaitMarking)
		{
			m_navMeshAgent.isStopped = false;
			m_state = State.Move;

			isMove = true;
			m_navMeshAgent.SetDestination(m_targetPoints[m_moveIndexInfo.x]);
			m_returnDestination = m_navMeshAgent.destination;
		}
	}
	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
		if (m_targetPoints == null || m_targetPoints.Length <= 1) return;
		
		if (MainGameManager.instance.resultState != MainGameManager.ResultState.Null)
		{
			m_navMeshAgent.enabled = false;
			return;
		}

		if (isLinked)
			PlayerAndTerritoryManager.instance.ReserveCalucrateTerritory(linkPlayerID);

		if (m_isWaitMarking)
		{
			if (m_isWaitMarkingEnd)
			{
				m_navMeshAgent.isStopped = false;
				m_isWaitMarking = false;
				m_isWaitMarkingEnd = false;
				m_moleAnimator.SetTrigger(m_cMoleAnimationEscapeMoveID);
				m_state = State.MoveStart;
				for (int i = 0; i < m_particleSystems.Length; ++i) m_particleSystems[i].Play();
			}
			return;
		}

		switch (m_state)
		{
			case State.Move:
				{
					if ((m_targetPoints[m_moveIndexInfo.x] - transform.position).sqrMagnitude < m_cRemainingCheckDistance)
					{
						ProceedTargetAndSetMoleAndParticles();

						isMove = false;
						m_timer.Start();
						m_state = State.Wait;
						m_navMeshAgent.isStopped = true;
						m_moleAnimator.SetTrigger(m_cMoleAnimationAppearID);
						
						for (int i = 0; i < m_particleSystems.Length; ++i) m_particleSystems[i].Stop();
					}

					break;
				}
			case State.Wait:
				{
					if (m_timer.elapasedTime >= m_waitSeconds)
					{
						m_state = State.MoveStart;
						m_moleAnimator.SetTrigger(m_cMoleAnimationEscapeMoveID);
						for (int i = 0; i < m_particleSystems.Length; ++i) m_particleSystems[i].Play();
					}

					break;
				}
			default:
				break;
		}
	}

    /// <summary>
    /// [LinkPoint] (Virtual)
    /// ポイントがリンクされた際にコールバックされる関数
    /// </summary>
    public override void LinkPoint()
	{
	}
	/// <summary>
	/// [UnlinkPoint] (Virtual)
	/// ポイントがリンク解除された際にコールバックされる関数
	/// </summary>
	public override void UnlinkPoint()
	{
	}
	public override void LinkMarkingStart()
	{
		m_isWaitMarking = true;
		m_navMeshAgent.isStopped = true;
		m_moleAnimator.SetTrigger(m_cMoleAnimationForceAppearID);

		for (int i = 0; i < m_particleSystems.Length; ++i) m_particleSystems[i].Stop();
	}
	public override void LinkMarkingEnd()
	{
		m_isWaitMarkingEnd = true;
	}



	void Start()
	{
		m_state = State.MoveStart;
		m_navMeshSpeed = m_navMeshAgent.speed;
		m_moleAnimator.SetTrigger(m_cMoleAnimationStartMoveID);
		m_moleAnimator.SetTrigger(m_cMoleAnimationEscapeMoveID);

		Vector3 absolute = m_mole.localPosition;
		absolute.y = 0.0f;
		m_moleDistance = absolute.magnitude;

		if (m_targetPoints != null  && m_targetPoints.Length > 1)
		{
			//Search positions
			NavMeshHit navMeshHit;
			for (int i = 0, length = m_targetPoints.Length; i < length; ++i)
			{
				if (NavMesh.SamplePosition(m_targetPoints[i], out navMeshHit, 2.0f, NavMesh.AllAreas))
					m_targetPoints[i] = navMeshHit.position;
				else
				{
#if UNITY_EDITOR
					Debug.LogWarning("Error!! MoveMarkPoint->Start, index[" + i + "] invalid position");
#endif
				}
			}

			//初期回転の設定
			m_firstRotations = new Quaternion[m_particleSystems.Length];
			for (int i = 0; i < m_particleSystems.Length; ++i)
			{
				m_firstRotations[i] = Quaternion.Euler(
					m_particleSystems[0].main.startRotationXMultiplier,
					m_particleSystems[0].main.startRotationYMultiplier,
					m_particleSystems[0].main.startRotationZMultiplier)
					* Quaternion.Inverse(m_particleAdministratorObject.rotation);
			}

			//一番近いindexを探索
			Vector3 position = transform.position;
			int minIndex = 0;
			float buf = 0.0f;
			float minValue = 100000.0f;
			for (int i = 0; i < m_targetPoints.Length; ++i)
			{
				buf = (m_targetPoints[i] - position).sqrMagnitude;
				if (minValue > buf)
				{
					minIndex = i;
					minValue = buf;
				}
			}
			//一番近いindexを設定
			m_moveIndexInfo.x = minIndex;

			//回転と位置の初期設定を行う->再生
			ProceedTargetAndSetMoleAndParticles();
			for (int i = 0; i < m_particleSystems.Length; ++i) m_particleSystems[i].Play();
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogWarning("Error!! MoveMarkPoint->Start, invalid target positions");
#endif
		}
	}

	void Update()
	{
		if (MainGameManager.instance.isPauseEnter)
		{
			m_navMeshAgent.speed = 0.0f;
			m_navMeshAgent.destination = transform.position;
		}
		else if (MainGameManager.instance.isPauseExit)
		{
			m_navMeshAgent.speed = m_navMeshSpeed;
			m_navMeshAgent.SetDestination(m_returnDestination);
		}
	}


	void ProceedTargetAndSetMoleAndParticles()
	{
		int oldIndex = m_moveIndexInfo.x;

		//Proceed target index
		m_moveIndexInfo.x += m_moveIndexInfo.y;
		if (m_moveIndexInfo.x < 0 || m_moveIndexInfo.x >= m_targetPoints.Length)
		{
			m_moveIndexInfo.y *= -1;
			m_moveIndexInfo.x += m_moveIndexInfo.y * 2;
		}

		//Set mole
		{
			m_toNextRotation = Quaternion.LookRotation((m_targetPoints[m_moveIndexInfo.x].ToYZero()
				- m_targetPoints[oldIndex].ToYZero()).normalized);

			m_mole.localPosition = (m_targetPoints[oldIndex].ToYZero()
				- m_targetPoints[m_moveIndexInfo.x].ToYZero()).normalized * m_moleDistance;
			m_mole.rotation = m_toNextRotation;
		}

		//Set Particle
		{
			for (int i = 0; i < m_particleSystems.Length; ++i)
			{
				m_mainModule = m_particleSystems[i].main;
				m_toNextRotationEuler = (m_firstRotations[i] * m_toNextRotation).eulerAngles * Mathf.Deg2Rad;

				m_mainModule.startRotationXMultiplier = m_toNextRotationEuler.x;
				m_mainModule.startRotationYMultiplier = m_toNextRotationEuler.y;
				m_mainModule.startRotationZMultiplier = m_toNextRotationEuler.z;
			}
		}
	}


	public override bool IsMovePoint()
	{
		return true;
	}

#if UNITY_EDITOR
	static readonly Vector3 m_dScale = new Vector3(0.1f, 5.0f, 0.1f);
	new void OnDrawGizmos()
	{
		base.OnDrawGizmos();

		for (int i = 0; i < m_targetPoints.Length; ++i)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(m_targetPoints[i], m_dScale);
			Gizmos.color = Color.red;

			if (i < m_targetPoints.Length - 1)
			{
				Vector3 from = m_targetPoints[i], to = m_targetPoints[i + 1];
				Gizmos.DrawLine(from, to);

				from.y += m_dScale.y * 0.5f;  to.y += m_dScale.y * 0.5f;
				Gizmos.DrawLine(from, to);

				from.y -= m_dScale.y;  to.y -= m_dScale.y;
				Gizmos.DrawLine(from, to);
			}
		}
	}
#endif
}
