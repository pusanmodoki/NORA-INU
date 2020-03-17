using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryManager : MonoBehaviour
{
	public static TerritoryManager instance { get; private set; } = null;
	
	[SerializeField, Range(0, 30)]
	int m_divisionSphere = 10;
	[SerializeField, Range(0, 10)]
	float m_sphereScaler = 3.0f;

	Vector3[] m_volumePoints = null;

	public void AddVolumePoints(List<Vector3> points)
	{
		for (int i = 0, count = points.Count; i < count; ++i)
			for (int k = 0, length = m_volumePoints.Length; k < length; ++k)
				points.Add(points[i] + (m_volumePoints[k] * m_sphereScaler));
	}

	void Awake()
	{
		if (instance == null) instance = this;

		m_volumePoints = new Vector3[m_divisionSphere];

		float angle = 0.0f, divisionAngle = Mathf.PI * 2.0f / m_divisionSphere;

		for (int i = 0; i < m_divisionSphere; ++i)
		{
			angle += divisionAngle;
			m_volumePoints[i] = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
		}
	}
	void OnDestroy()
	{
		if (instance == this) instance = this;
	}
}
