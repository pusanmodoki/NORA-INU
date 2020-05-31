using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextEvent : MonoBehaviour
{
    Dictionary<string, GameObject> m_eventTexts = new Dictionary<string, GameObject>();

    [SerializeField]
    GameObject activeText = null;

    private void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            m_eventTexts.Add(transform.GetChild(i).name, transform.GetChild(i).gameObject);
        }
    }


    private void Update()
    {
        switch (OneMarkSceneManager.instance.nowStageSceneIndex.y)
        {
            case 1:
                {
                    Stage1Event();
                    break;
                }
            case 2:
                {
                    Stage2Event();
                    break;
                }
        }
    }

    void Stage1Event()
    {
        if(!m_eventTexts["text1-2"].activeSelf && PlayerAndTerritoryManager.instance.mainPlayer.allTerritorys.Count == 2)
        {
            activeText.SetActive(false);
            m_eventTexts["text1-2"].SetActive(true);
            activeText = m_eventTexts["text1-2"];
        }
        if (!m_eventTexts["text1-3"].activeSelf && MainGameManager.instance.m_resultState == MainGameManager.ResultState.GameClear)
        {
            activeText.SetActive(false);
            m_eventTexts["text1-3"].SetActive(true);
            activeText = m_eventTexts["text1-3"];
        }
    }



    void Stage2Event()
    {
        if(m_eventTexts["text2-1"].activeSelf)
        {
            foreach(var servant in ServantManager.instance.servantByMainPlayer)
            {
                if (!servant.linkMarkPoint)
                {
                    return;
                }
            }
            m_eventTexts["text2-1"].SetActive(false);
            m_eventTexts["text2-2"].SetActive(true);
        }
        else if (m_eventTexts["text2-2"].activeSelf)
        {
            foreach (var servant in ServantManager.instance.servantByMainPlayer)
            {
                if (!servant.linkMarkPoint)
                {
                    m_eventTexts["text2-2"].SetActive(false);
                    m_eventTexts["text2-3"].SetActive(true);
                }
            }
        }
        else if (m_eventTexts["text2-3"].activeSelf && 
            (MainGameManager.instance.m_resultState == MainGameManager.ResultState.GameClear ||
            MainGameManager.instance.m_resultState == MainGameManager.ResultState.GameEnd))
        {
            m_eventTexts["text2-3"].SetActive(false);
            m_eventTexts["text2-4"].SetActive(true);
        }
    }
    void Stage3Event()
    {
    }
    void Stage4Event()
    {
    }
}
