﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    [SerializeField]
    bool m_isAnimation = true;

    public bool isAnimation { get { return m_isAnimation; } }

    public void GameStart()
    {
        m_isAnimation = false;

        PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableInput = true;
    }
}