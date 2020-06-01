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
	ParticleSystem[] m_particleSystems = null;
	[SerializeField]
	Vector3[] m_targetPoints = null;

	Quaternion m_toNextRotation = Quaternion.identity;
	Vector3 m_toNextRotationEuler = Vector3.zero;
	Vector3 m_toNextNormalized = Vector3.zero;
	Vector2Int m_moveIndexInfo = new Vector2Int(0, 1);
	Timer m_timer = new Timer();
	State m_state = State.MoveStart;
	float m_moleDistance = 0.0f;
	bool m_isWaitMarking = false;
	bool m_isWaitMarkingEnd = false;


	public void AnimationEscapeCallback()
	{
		if (m_state == State.MoveStart && !m_isWaitMarking)
		{
			m_navMeshAgent.isStopped = false;
			m_state = State.Move;
			m_navMeshAgent.SetDestination(m_targetPoints[m_moveIndexInfo.x]);
		}
	}
	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
		if (m_targetPoints == null || m_targetPoints.Length <= 1) return;

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
					isMove = true;

					if ((m_targetPoints[m_moveIndexInfo.x] - transform.position).sqrMagnitude < m_cRemainingCheckDistance)
					{
						int old = m_moveIndexInfo.x;

						m_moveIndexInfo.x += m_moveIndexInfo.y;
						if (m_moveIndexInfo.x < 0 || m_moveIndexInfo.x >= m_targetPoints.Length)
						{
							m_moveIndexInfo.y *= -1;
							m_moveIndexInfo.x += m_moveIndexInfo.y * 2;
						}
		
						m_toNextNormalized = (m_targetPoints[m_moveIndexInfo.x].ToYZero() - m_targetPoints[old].ToYZero()).normalized;
						m_toNextRotation = Quaternion.LookRotation(m_toNextNormalized);
						m_toNextRotationEuler = Quaternion.Inverse(m_toNextRotation).eulerAngles;
						m_toNextRotationEuler.y += 180;
						m_toNextRotationEuler *= Mathf.Deg2Rad;

						m_mole.localPosition = (m_targetPoints[old].ToYZero()
							- m_targetPoints[m_moveIndexInfo.x].ToYZero()).normalized * m_moleDistance;

						m_mole.rotation = m_toNextRotation;
						for (int i = 0; i < m_particleSystems.Length; ++i)
						{
							var main = m_particleSystems[i].main;
							main.startRotationXMultiplier = m_toNextRotationEuler.x;
							main.startRotationYMultiplier = m_toNextRotationEuler.y;
							main.startRotationZMultiplier = m_toNextRotationEuler.z;
						}

						m_timer.Start();
						m_state = State.Wait;
						m_moleAnimator.SetTrigger(m_cMoleAnimationAppearID);
						m_navMeshAgent.isStopped = true;
						isMove = false;

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
		m_moleAnimator.SetTrigger(m_cMoleAnimationStartMoveID);
		m_moleAnimator.SetTrigger(m_cMoleAnimationEscapeMoveID);
		for (int i = 0; i < m_particleSystems.Length; ++i) m_particleSystems[i].Play();

		Vector3 absolute = m_mole.localPosition;
		absolute.y = 0.0f;
		m_moleDistance = absolute.magnitude;

		if (m_targetPoints != null  && m_targetPoints.Length > 1)
		{
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
			
			m_mole.localPosition = (m_targetPoints[1].ToYZero() - m_targetPoints[0].ToYZero()).normalized * m_moleDistance;

			m_mole.LookAt(m_targetPoints[0].ToYManual(m_mole.position.y));
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogWarning("Error!! MoveMarkPoint->Start, invalid target positions");
#endif
		}

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
				Vector3 toNext = m_targetPoints[i + 1] - m_targetPoints[i];
				Gizmos.DrawWireCube(
					new Vector3(m_targetPoints[i].x + toNext.x * 0.5f, m_targetPoints[i].y, m_targetPoints[i].z + toNext.z * 0.5f),
					new Vector3(toNext.x, 5.0f, toNext.z));
			}
		}
	}
#endif
}
