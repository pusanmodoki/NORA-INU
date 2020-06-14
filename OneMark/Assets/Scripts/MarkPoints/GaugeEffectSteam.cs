using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeEffectSteam : MonoBehaviour
{
	[SerializeField]
	BaseMarkPoint m_markPoint = null;

	[SerializeField]
	MarkPointGauge m_gauge = null;

	[SerializeField]
	ParticleSystem m_effect = null;

	bool m_isPlay = false;
	// Update is called once per frame
	void Update()
	{
		float t = m_markPoint.effectiveCounter01;
		if ((t <= 0 || t >= 1) && m_isPlay)
		{
			m_isPlay = false;
			m_effect.Stop();
		}
		else if (t > 0 && t < 1 && !m_isPlay)
		{
			m_isPlay = true;
			m_effect.Play();
		}

		Vector3 pos = transform.localPosition;

		pos.y = (1 - t) * m_gauge.m_maxHeight * 0.8f;

		transform.localPosition = pos;
	}
}
