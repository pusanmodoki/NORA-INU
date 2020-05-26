using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour
{
    [SerializeField]
    private int nowSelectIndex = 0;

    [SerializeField]
    private bool isInput = false;

    [SerializeField]
    private int stageNo = 4;

    List<Image> button = new List<Image>();

    float seconds = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < stageNo; ++i)
        {
            button.Add(transform.GetChild(i).GetComponent<Image>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInput)
        {
            InputUpAndDown();
        }

        ButtonSelect();

    }

    void InputUpAndDown()
    {
        float value = 0.0f;
        if (Input.GetButton("Vertical"))
        {
            value = Input.GetAxisRaw("Vertical");
            if (value == -1.0f)
            {// 下
                if (nowSelectIndex == stageNo - 1) return;
                ++nowSelectIndex;
                isInput = true;
            }
            else
            {// 上
                if (nowSelectIndex == 0) return;
                --nowSelectIndex;
                isInput = true;
            }
        }

        // 決定
        if (Input.GetButton("Fire2"))
        {
            transform.GetChild(nowSelectIndex).GetChild(0).gameObject.SetActive(true);
            OneMarkSceneManager.instance.MoveStageScene(StageSelectManager.instance.worldNum);
        }

    }

    void ButtonSelect()
    {

        if (nowSelectIndex == 0)
        {
            button[0].color = Color.white;
            button[1].color = Color.gray;
        }
        else if (nowSelectIndex == 1 || nowSelectIndex == 2) 
        {
            button[nowSelectIndex].color = Color.white;
            button[nowSelectIndex - 1].color = Color.gray;
            button[nowSelectIndex + 1].color = Color.gray;
        }
        else
        {
            button[nowSelectIndex].color = Color.white;
            button[nowSelectIndex - 1].color = Color.gray;
        }

        seconds += Time.deltaTime;
        if (seconds >= 0.357f)
        {
            seconds = 0.0f;
            isInput = false;
        }

    }
}
