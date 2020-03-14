using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeedChanger : MonoBehaviour
{
	public float targetSpeed { get { return m_targetSpeed; } }
	public bool isGradientMode { get; private set; } = false;

	public bool isEnabledChangeSpeed
	{
		get { return m_isEnabledChangeSpeed; }
		set
		{
			m_isEnabledChangeSpeed = value;
			if (value) Update();
			else m_navMeshAgent.speed = m_targetSpeed;
		}
	}

	[Header("Reference"), SerializeField]
	NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	BoxCastFlags m_groundFlags = null;
	[SerializeField]
	BoxCastFlags m_gradientFlags = null;

	[Header("Speed of nav mesh"), SerializeField]
	float m_targetSpeed = 1.0f;
	[SerializeField]
	float m_minSpeed = 0.1f;
	[SerializeField]
	float m_maxSpeed = 20.0f;
	[SerializeField, Range(0.0f, 1.0f)]
	float m_gradientDecelerationRatio = 0.5f;

	[Header("Acceleration per seconds"), SerializeField]
	float m_gradientAcceleration = 1.0f;
	[SerializeField]
	float m_maxAcceleration = 10.0f;
	[SerializeField, Range(0.0f, 3.0f)]
	float m_decelerationSeconds = 0.1f;

	[Header("Other"), SerializeField, Range(0.0f, 0.1f)]
	float m_gradientModeSeconds = 0.1f;
	[SerializeField]
	bool m_isEnabledChangeSpeed = true;

	Timer m_gradientModeTimer = new Timer();
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

	}

	// Update is called once per frame
	void Update()
    {
		if (!(isEnabledChangeSpeed & m_groundFlags.isStay)) return;

		float dotGradient = Vector3.Dot(m_groundFlags.boxCastResult.normal, Vector3.up);

		CheckGradient(dotGradient);

		float targetSpeed = CalculateSpeed(dotGradient);

		m_navMeshAgent.speed = targetSpeed;
	}

	void CheckGradient(float dotGradient)
	{
		//傾斜ならStart
		if (dotGradient < 1.0f - Mathf.Epsilon)
		{
			if (!m_gradientModeTimer.isStart)
				m_gradientModeTimer.Start();

			if (m_gradientModeTimer.elapasedTime > m_gradientModeSeconds)
				isGradientMode = true;
		}
		else
		{
			m_gradientModeTimer.Stop();
			isGradientMode = false;
		}
	}

	float CalculateSpeed(float dotGradient)
	{
		if (!isGradientMode)
		{
			m_nowAcceleration -= (m_nowAcceleration / m_decelerationSeconds) * Time.deltaTime;

			if (m_nowAcceleration < 0.001f)
				m_nowAcceleration = 0.0f;

			return Mathf.Clamp(m_targetSpeed + m_nowAcceleration, m_minSpeed, m_maxSpeed);
		}


		if (!m_gradientFlags.isStay)
		{
			m_nowAcceleration += m_gradientAcceleration * Time.deltaTime;

			if (m_nowAcceleration > m_maxAcceleration)
				m_nowAcceleration = m_maxAcceleration;

			return Mathf.Clamp(m_targetSpeed + m_nowAcceleration, m_minSpeed, m_maxSpeed);
		}
		else
		{
			m_nowAcceleration -= (m_nowAcceleration / m_decelerationSeconds) * Time.deltaTime;

			if (m_nowAcceleration < 0.001f)
				m_nowAcceleration = 0.0f;

			return Mathf.Clamp(dotGradient * m_gradientDecelerationRatio 
				* m_targetSpeed + m_nowAcceleration, m_minSpeed, m_maxSpeed);
		}
	}
}
