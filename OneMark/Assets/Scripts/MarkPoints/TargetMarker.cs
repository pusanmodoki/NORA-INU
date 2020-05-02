﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    private float rotateSpeed = 1.0f;

    private List<MeshRenderer> m_renderers = new List<MeshRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            m_renderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        TurningEffect();
    }

    private void TurningEffect()
    {
        transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
        if(target == null)
        {
            OffEffect();
            return;
        }

        transform.position = target.transform.position;
        OnEffect();
    }

    private void OffEffect()
    {
        foreach(var renderer in m_renderers)
        {
            renderer.enabled = false;
        }
    }

    private void OnEffect()
    {
        foreach (var renderer in m_renderers)
        {
            renderer.enabled = true;
        }
    }

}
