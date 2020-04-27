using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMeshController : MonoBehaviour
{
	public NavMeshAgent navMeshAgent { get { return m_navMeshAgent; } }
	public GameObject targetObject { get { return m_targetObject; } }

	[SerializeField]
	NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	NavMeshSurface m_navMeshSurface = null;
	[SerializeField]
	GameObject m_targetObject = null;

	[SerializeField, Space]
	float m_bakeInterval = 0.1f;
	[SerializeField, Space]
	float m_setTargetPointInterval = 0.05f;

	Timer m_bakeIntervalTimer = new Timer();
	Timer m_setTargetIntervalTimer = new Timer();

    // Start is called before the first frame update
    void Start()
	{
		m_bakeIntervalTimer.Start();
		m_setTargetIntervalTimer.Start();
	}

	// Update is called once per frame
	void Update()
    {
        if (m_bakeIntervalTimer.elapasedTime > m_bakeInterval)
		{
			m_navMeshSurface.BuildNavMesh();
			m_bakeIntervalTimer.Start();
		}

		if (m_setTargetIntervalTimer.elapasedTime > m_bakeInterval)
		{
			m_navMeshAgent.destination = m_targetObject.transform.position;
			m_setTargetIntervalTimer.Start();
		}
	}
}
