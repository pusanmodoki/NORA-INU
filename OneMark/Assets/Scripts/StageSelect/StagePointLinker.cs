using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePointLinker : MonoBehaviour
{
    [SerializeField]
    private int nowSelectStageID = 0;

    [SerializeField]
    private SelectSoundPlayer SEPlayer = null;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        nowSelectStageID = 0;
        transform.GetChild(nowSelectStageID).GetComponent<SelectIcon>().m_isSelected = true;
    }

    private void Update()
    {
        SelectInput();

    }

    private void SelectInput()
    {
        float value = 0.0f;

        if (Input.GetButtonDown("Fire2"))
        {
            SEPlayer.EnterPlay();
            transform.GetChild(nowSelectStageID).GetComponent<SelectIcon>().ThisSelectSceneTrans();
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            transform.parent.parent.GetComponent<StageSlide>().m_isWorldSelect = true;
            gameObject.SetActive(false);
        }
        if (Input.GetButtonDown("Horizontal"))
        {
            value = Input.GetAxisRaw("Horizontal");
            if (value == -1.0f)
            {
                if (nowSelectStageID == 0) return;
                transform.GetChild(nowSelectStageID).GetComponent<SelectIcon>().m_isSelected = false;
                --nowSelectStageID;
                transform.GetChild(nowSelectStageID).GetComponent<SelectIcon>().m_isSelected = true;
                SEPlayer.SelectPlay();
            }
            else
            {
                if (nowSelectStageID == transform.childCount - 1) return;
                transform.GetChild(nowSelectStageID).GetComponent<SelectIcon>().m_isSelected = false;
                ++nowSelectStageID;
                transform.GetChild(nowSelectStageID).GetComponent<SelectIcon>().m_isSelected = true;
                SEPlayer.SelectPlay();
            }
        }

    }

    private void SelectedStageFlag()
    {
        
    }
}
