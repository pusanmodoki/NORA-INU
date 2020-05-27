﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCovering : BaseMovePoint
{
    [SerializeField]
    Transform m_lookPoint = null;

    [SerializeField]
    float m_late = 1.0f;

    float m_t = 0.0f;

    float m_stageHeight = 0.0f;
    float m_stageWidth = 0.0f;

    GameObject m_player = null;



    // Start is called before the first frame update
    void Start()
    {
        m_player = PlayerAndTerritoryManager.instance.mainPlayer.gameObject;

        m_stageHeight = MainGameManager.instance.stageSize.y;
        m_stageWidth = MainGameManager.instance.stageSize.x;
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent)
        {
            transform.SetParent(null);
        }
        SetPosition();
    }

    public override void MovePointUpdate(Vector3 _vec)
    {
        Vector3 point = m_cover.startPoint.position + _vec;
        transform.position = Vector3.Lerp(transform.position, point, m_late);

        Look();
    }

    private void SetPosition()
    {
        m_cover.t = m_player.transform.position.z / m_stageHeight;
    }

    private void Look()
    {
        Vector3 point = Vector3.zero;
        point.x = m_stageWidth / 2.0f;
        point.z = m_stageHeight / 2.0f;

        transform.LookAt(point);

        
    }
}
