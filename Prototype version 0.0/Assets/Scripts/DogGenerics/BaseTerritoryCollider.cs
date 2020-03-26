using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTerritoryCollider : MonoBehaviour
{
	public bool isLockDetection
	{
		get { return m_isDrawingLockDetection; }
		set
		{
			m_isDrawingLockDetection = value;
			if (value) m_intervalTimer.Stop();
			else m_intervalTimer.Start();
		}
	}

	public bool isEnter { get; private set; } = false;
	public bool isStay { get; private set; } = false;
	public bool isExit { get; private set; } = false;

	public bool isDetectionFrame { get; private set; } = false;

	protected float m_cHitDistance = 10000.0f;

	[SerializeField, Range(0.1f, 10.0f)]
	protected float m_radius = 0.0f;
	[SerializeField]
	float m_detectionInterval = 0.1f;

	[SerializeField]
	bool m_isDrawingLockDetection = false;

	Timer m_intervalTimer = new Timer();

	// Start is called before the first frame update
	protected void Start()
    {
		if (!m_isDrawingLockDetection)
			m_intervalTimer.Start();
	}

	// Update is called once per frame
	protected void Update()
	{
		if (isLockDetection)
		{
			isDetectionFrame = false;
			return;
		}

		if (m_intervalTimer.elapasedTime >= m_detectionInterval)
		{
			m_intervalTimer.Start();
			isDetectionFrame = true;
		}
		else
		{
			isDetectionFrame = false;
			isEnter = false;
			isExit = false;
		}
	}

	protected void SetHitFlags(bool flag)
	{
		bool isOldStay = isStay;
		isStay = flag;

		if ((isOldStay ^ isStay) & isStay)
		{
			isEnter = true;
			isExit = false;
		}
		else if ((isOldStay ^ isStay) & isOldStay)
		{
			isEnter = false;
			isExit = true;
		}

	}

#if UNITY_EDITOR
	static readonly Color m_cDrawColor = new Color(0.9f, 0.6f, 0.6f);

	void OnDrawGizmos()
	{
		Gizmos.color = m_cDrawColor;
		Gizmos.DrawWireSphere(transform.position, m_radius);
	}
#endif
}
