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

    [SerializeField, Range(0.0f, 1.0f)]
    float m_safetyLine = 0.8f;

    [SerializeField, Range(0.0f, 1.0f)]
    float m_dangerLine = 0.3f;

    [SerializeField]
    float m_rate = 20.0f;

    [SerializeField]
    Color m_safetyColor = Color.white;
    [SerializeField]
    Color m_color = Color.white;
    [SerializeField]
    Color m_dangerColor = Color.white;


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;

        float t = m_markPoint.effectiveCounter01;

        pos.y = t * m_gauge.m_maxHeight * 0.8f;

        transform.localPosition = pos;
        ParticleSystem.EmissionModule emission = m_effect.emission;
        if(t <= 0 || t >= 1)
        {
            emission.rateOverTime = 0.0f;
        }
        else
        {
            emission.rateOverTime = m_rate;
        }

        ParticleSystem.MainModule main = m_effect.main;

        if(t > m_safetyLine)
        {
            main.startColor = m_safetyColor;
        }
        else if(t < m_dangerLine)
        {
            main.startColor = m_dangerColor;
        }
        else
        {
            main.startColor = m_color;
        }
    }
}
