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
	BoxCastFlags m_gradientFlags = null;
	[SerializeField]
	float m_targetSpeed = 1.0f;
	[SerializeField]
	float m_gradientAcceleration = 1.0f;
	[SerializeField]
	float m_maxAcceleration = 10.0f;
	[SerializeField]
	bool m_isEnabledChangeSpeed = true;

	Timer m_gradientModeTime = new Timer();
	float m_nowSpeed = 0.0f;
	float m_nowAcceleration = 0.0f;

	void Start()
	{
#if UNITY_EDITOR
		if (m_navMeshAgent == null)
		{
			Debug.LogError("Error!! SpeedChanger->Awake NavMeshAgent == null");
			throw new System.Exception();
		}
		if (m_groundFlags == null)
		{
			Debug.LogError("Error!! SpeedChanger->Awake GroundFlags == null");
			throw new System.Exception();
		}
		if (m_gradientFlags == null)
		{
			Debug.LogError("Error!! SpeedChanger->Awake GradientFlags == null");
			throw new System.Exception();
		}
#endif

		m_nowSpeed = m_navMeshAgent.speed;	
	}

	// Update is called once per frame
	void Update()
    {
		if (!(isEnabledChangeSpeed & m_groundFlags.isStay)) return;

		float targetSpeed = CalculateSpeed();

		m_navMeshAgent.speed = targetSpeed;

		Debug.Log(targetSpeed);
	}

	float CalculateSpeed()
	{
		if (!m_gradientFlags.isStay)
		{
			m_nowAcceleration += m_gradientAcceleration * Time.deltaTime;

			if (m_nowAcceleration > m_maxAcceleration)
				m_nowAcceleration = m_maxAcceleration;

			return 1.0f * m_targetSpeed + m_nowAcceleration;
		}
		else
		{
			m_nowAcceleration -= m_nowAcceleration * 1 * Time.deltaTime;
			if (m_nowAcceleration < 0.01f)
				m_nowAcceleration = 0.0f;

			return Mathf.Max(1.0f, Vector3.Dot(m_groundFlags.boxCastResult.normal, Vector3.up))
				* m_targetSpeed + m_nowAcceleration;
		}
	}
}
