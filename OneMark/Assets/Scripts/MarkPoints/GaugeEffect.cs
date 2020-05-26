using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeEffect : MonoBehaviour
{
    [SerializeField]
    BaseMarkPoint m_markPoint = null;

    [SerializeField]
    MarkPointGauge m_gauge = null;

    [SerializeField]
    ParticleSystem m_effect = null;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;

        float t = m_markPoint.effectiveCounter / MarkPointManager.instance.effectiveMaxLimiter;

        pos.y = t * m_gauge.m_maxHeight * 0.8f;

        transform.localPosition = pos;
        ParticleSystem.EmissionModule emission = m_effect.emission;
        if(t <= 0 || t >= 1)
        {
            emission.rateOverTime = 0.0f;
        }
        else
        {
            emission.rateOverTime = 100.0f;
        }
    }
}
