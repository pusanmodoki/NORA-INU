using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeedChanger : MonoBehaviour
{
	public bool isEnabledChangeSpeed
	{
		get { return m_isEnabledChangeSpeed; }
		set { m_isEnabledChangeSpeed = value; m_nowSpeed = m_navMeshAgent.speed; }
	}

	[SerializeField]
	NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	BoxCastFlags m_groundFlags = null;
	[SerializeField]
	float m_targetSpeed = 1.0f;
	[SerializeField]
	bool m_isEnabledChangeSpeed = true;

	float m_nowSpeed = 0.0f;

	void Start()
	{
		if (m_navMeshAgent == null)
		{
			Debug.LogError("Error!! SpeedChanger->Awake NavMeshAgent == null");
			return;
		}
		if (m_groundFlags == null)
		{
			Debug.LogError("Error!! SpeedChanger->Awake GroundFlags == null");
			return;
		}

		m_nowSpeed = m_navMeshAgent.speed;	
	}

	// Update is called once per frame
	void Update()
    {
        if (isEnabledChangeSpeed & m_groundFlags.isStay)
		{
			float targetSpeed = m_targetSpeed * Vector3.Dot(m_groundFlags.boxCastResult.normal, Vector3.up);

			Debug.Log(Vector3.Angle(m_groundFlags.boxCastResult.normal, Vector3.up));

		}
    }
}
