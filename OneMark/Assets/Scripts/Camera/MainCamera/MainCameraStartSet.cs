using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraStartSet : MonoBehaviour
{
    [SerializeField]
    Transform m_lookPoint = null;

    [SerializeField]
    Transform m_movePoint = null;


	[SerializeField]
	float m_moveSeconds = 1.0f;

	[SerializeField]
	float m_moveLookPointSeconds = 1.0f;

	Timer m_timer = new Timer();
	bool m_isMoving = false;

	Vector3 m_startLookPoint = Vector3.zero;

	private void Start()
	{
		transform.SetParent(null);
	}

	private void LateUpdate()
    {
		if (m_isMoving) { Interpolation(); }
    }

    private void Interpolation()
    {
        Vector3 vec = Vector3.zero;

		vec = Vector3.Lerp(transform.position, m_movePoint.transform.position, m_timer.elapasedTime / m_moveSeconds);
		transform.position = vec;

		vec = Vector3.zero;

		vec = Vector3.Lerp(m_startLookPoint, m_lookPoint.transform.position, m_timer.elapasedTime / m_moveLookPointSeconds);
		transform.LookAt(vec);
		m_startLookPoint = vec;

		if(m_timer.elapasedTime >= m_moveSeconds && m_timer.elapasedTime >= m_moveLookPointSeconds)
		{
			m_isMoving = false;
			m_timer.Stop();
		}
	}

	public void StartMove()
	{
		m_isMoving = true;
		m_timer.Start();
		m_startLookPoint = transform.position + transform.forward;
	}
}
