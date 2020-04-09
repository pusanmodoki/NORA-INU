using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class DogRushingAndMarking : BaseDogAIFunction
{
	enum State
	{
		Rushing,
		Rotation,
		Marking,
	}

	[SerializeField]
	float m_rushingAddAcceleration = 10.0f;
	[SerializeField]
	float m_rushingArrivalDistance = 0.8f;

	[SerializeField]
	Vector3 m_markingRotation = Vector3.zero;
	[SerializeField]
	float m_rotationSpeed = 0.9f;
	[SerializeField]
	float m_rotationSeconds = 1.0f;

	[SerializeField]
	float m_markingSeconds = 0.9f;

	BaseMarkPoint m_markPoint = null;
	Vector3 m_markPointPosition= Vector3.zero;
	LayerMaskEx m_markPointLayerMask = 0;

	Quaternion m_targetRotation = Quaternion.identity;
	State m_state;
	
	public void SetAdvanceInformation(BaseMarkPoint markPoint, Vector3 position)
	{
		m_markPoint = markPoint;
		m_markPointPosition = position;
		m_markPointLayerMask.SetValue(markPoint.gameObject);
	}

	public override void AIBegin(BaseAIFunction beforeFunction)
	{
		if (m_markPoint == null) return;

		SetUpdatePosition(true);
		navMeshAgent.destination = m_markPointPosition;
		dogAIAgent.speedChanger.SetManualAcceleration(m_rushingAddAcceleration);
		m_state = State.Rushing;
	}

	public override void AIEnd(BaseAIFunction nextFunction)
	{
		m_markPoint = null;
	}

	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		if (m_markPoint == null)
		{
			EndAIFunction(updateIdentifier);
			return;
		}
		else if (m_markPoint.isLinked)
		{
			EndAIFunction(updateIdentifier);
			return;
		}

		switch (m_state)
		{
			case State.Rushing:
				{
					var hitCollisions = Physics.OverlapSphere(transform.position, m_rushingArrivalDistance, m_markPointLayerMask);

					for (int i = 0, length = hitCollisions.Length; i < length; ++i)
					{
						if (hitCollisions[i].gameObject.GetInstanceID()
							== m_markPoint.gameObject.GetInstanceID())
						{
							navMeshAgent.updatePosition = false;
							navMeshAgent.updateRotation = false;
							dogAIAgent.speedChanger.SetManualAcceleration(0.0f);

							m_targetRotation = Quaternion.LookRotation(
								(m_markPoint.transform.position - transform.position).normalized, transform.up) * Quaternion.Euler(m_markingRotation);
							m_state = State.Rotation;
							timer.Start();
							break;
						}
					}

					break;
				}

			case State.Rotation:
				{
					if (timer.elapasedTime >= m_rotationSeconds)
					{
						m_state = State.Marking;
						timer.Start();
					}

					transform.rotation =
						Quaternion.Slerp(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
					break;
				}

			case State.Marking:
				{
					if (timer.elapasedTime >= m_markingSeconds)
					{
						m_markPoint.LinkPlayer(dogAIAgent.linkPlayer, dogAIAgent);
						dogAIAgent.SetSitAndStay(true, m_markPoint);
						EndAIFunction(updateIdentifier);
					}

					break;
				}
			default:
				break;
		}
	}
}
