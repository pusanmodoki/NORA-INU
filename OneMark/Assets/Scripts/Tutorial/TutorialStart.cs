using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStart : MonoBehaviour
{
    [SerializeField]
    StageStart m_stageStart = null;

    [SerializeField]
    GameObject m_tutorialUI = null;

    [SerializeField]
    TutorialTextEvent text = null;

    // Update is called once per frame
    void Update()
    {
        if (!m_stageStart.isAnimation && !m_tutorialUI.activeSelf) 
        {
            m_tutorialUI.SetActive(true);
			PlayerAndTerritoryManager.instance.mainPlayer.input.StartDisableInput(out text.inputID);
            text.m_isTutorialInput = true;
        }
    }
}
