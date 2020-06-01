using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [SerializeField]
    private int nowSelectIndex = 0;

    [SerializeField]
    private bool isInput_Tate = false;

    [SerializeField]
    private int stageNo = 4;

    List<Image> button = new List<Image>();

    [SerializeField]
    private SelectSoundPlayer se = null;

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
        if (!isInput_Tate)
        {
            InputUpAndDown();
        }

        ButtonSelect();

    }

    void InputUpAndDown()
    {
        float value = 0.0f;
        if (Input.GetButtonDown("Vertical"))
        {
            se.SelectPlay();

            value = Input.GetAxisRaw("Vertical");
            if (value == -1.0f)
            {// 下
                if (nowSelectIndex == stageNo - 1) return;
                ++nowSelectIndex;
                isInput_Tate = true;
            }
            else
            {// 上
                if (nowSelectIndex == 0) return;
                --nowSelectIndex;
                isInput_Tate = true;
            }
        }

        // 決定
        if (Input.GetButtonDown("Fire2"))
        {
            se.EnterPlay();

            //Debug.Log(StageSelectManager.instance.worldNum +1);
            //Debug.Log(nowSelectIndex);

            transform.GetChild(nowSelectIndex).GetChild(0).gameObject.SetActive(true);
         //   OneMarkSceneManager.instance.MoveStageScene(new Vector2Int(StageSelectManager.instance.worldNum + 1, nowSelectIndex + 1));
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

        isInput_Tate = false;

    }
}
