using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Marker : MonoBehaviour
{
	enum State
	{
		Standby,
		StartRotation,
	}

	class MarkingObjectInfo
	{
		public MarkingObjectInfo(GameObject gameObject, 
			MarkingMessage message, Transform thisTransform, Vector3 goalRotation)
		{
			this.gameObject = gameObject;
			this.message = message;
			this.goalRotation = Quaternion.LookRotation((gameObject.transform.position - thisTransform.position).normalized, thisTransform.up) * Quaternion.Euler(goalRotation);
			this.position = gameObject.transform.position;
		}

		public GameObject gameObject;
		public MarkingMessage message;
		public Quaternion goalRotation;
		public Vector3 position;
	}

	public float attack { get { return m_attack; } }
	public bool isPossibleMarking { get; private set; } = false;
	public bool isNowMarking { get { return m_state != State.Standby; } }

	[SerializeField]
	GameObject m_rotationApplyObject = null;
	[SerializeField]
	Rigidbody m_rigidbody = null;
	[SerializeField]
	LayerMaskEx m_markingMask = 0;
	[SerializeField]
	Vector3 m_markingRotation = Vector3.zero;
	[SerializeField]
	float m_attack = 1.0f;
	[SerializeField]
	float m_possibleMarkingRadius = 1.0f;
	[SerializeField]
	float m_rotationSpeed = 0.9f;
	[SerializeField]
	float m_rotationTime = 1.0f;

	State m_state = State.Standby;
	MarkingObjectInfo m_markingObjectInfo = null;
	Timer m_timer = new Timer();

	void Update()
	{
		if (m_state == State.Standby)
			isPossibleMarking = Physics.CheckSphere(transform.position, m_possibleMarkingRadius);
		else
			isPossibleMarking = false;

		if (Input.GetKeyDown(KeyCode.V))
			RunMarking();
	}

	void LateUpdate()
	{
		
		switch(m_state)
		{
			case State.StartRotation:
				if (m_timer.elapasedTime >= m_rotationTime)
				{
					m_state = State.Standby;
					Marking();
					break;
				}

				m_rotationApplyObject.transform.rotation = 
					Quaternion.Slerp(transform.rotation, m_markingObjectInfo.goalRotation, m_rotationSpeed * Time.deltaTime);
				//m_rigidbody.velocity = Vector3.zero;
				//m_rigidbody.angularVelocity = Vector3.zero;

				break;
			default:
				break;
		}
	}

	public bool RunMarking()
	{
		if (!isPossibleMarking)
			return false;

		var collisions = Physics.OverlapSphere(transform.position,
			m_possibleMarkingRadius, m_markingMask);

		foreach (var e in collisions)
		{
			var component = e.gameObject.GetComponent<MarkingMessage>();
			if (component != null)
			{
				m_markingObjectInfo = new MarkingObjectInfo(e.gameObject, 
					component, transform, m_markingRotation);
				break;
			}
		}

		if (m_markingObjectInfo == null)
			return false;

		m_state = State.StartRotation;
		m_timer.Start();

		return true;
	}

	void Marking()
	{
		m_markingObjectInfo.message.SendMessage(this);
	}
}
