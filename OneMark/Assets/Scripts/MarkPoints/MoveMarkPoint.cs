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

	[SerializeField, Space]
	NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	Animator m_moleAnimator = null;
	[SerializeField]
	float m_waitSeconds = 1.0f;
	[SerializeField]
	Vector3[] m_targetPoints = null;

	Vector2Int m_moveIndexInfo = new Vector2Int(0, 1);
	Timer m_timer = new Timer();
	State m_state = State.MoveStart;
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
						m_moveIndexInfo.x += m_moveIndexInfo.y;
						if (m_moveIndexInfo.x < 0 || m_moveIndexInfo.x >= m_targetPoints.Length)
						{
							m_moveIndexInfo.y *= -1;
							m_moveIndexInfo.x += m_moveIndexInfo.y * 2;
						}

						m_timer.Start();
						m_state = State.Wait;
						m_moleAnimator.SetTrigger(m_cMoleAnimationAppearID);
						m_navMeshAgent.isStopped = true;
						isMove = false;
					}

					break;
				}
			case State.Wait:
				{
					if (m_timer.elapasedTime >= m_waitSeconds)
					{
						m_state = State.MoveStart;
						m_moleAnimator.SetTrigger(m_cMoleAnimationEscapeMoveID);
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
	}
	public override void LinkMarkingEnd()
	{
		m_isWaitMarkingEnd = true;
	}



	void Start()
	{
		m_state = State.MoveStart;
		m_moleAnimator.SetTrigger(m_cMoleAnimationEscapeMoveID);

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
