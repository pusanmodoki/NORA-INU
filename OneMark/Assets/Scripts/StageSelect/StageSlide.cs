using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSlide : MonoBehaviour
{
    [SerializeField]
    private int nowSelectIndex = 0;

    [SerializeField]
    private float interval = 20.0f;

    [SerializeField]
    private float scrollSpeed = 1.0f;

    [SerializeField]
    private bool isScroll = false;

    [SerializeField]
    private bool isWorldSelect = true;
    public bool m_isWorldSelect { set { isWorldSelect = value; } }

    [SerializeField]
    private SelectSoundPlayer SEPlayer = null;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            Vector3 localPos = transform.GetChild(i).GetComponent<RectTransform>().localPosition;
            localPos.x = i * interval;
            transform.GetChild(i).GetComponent<RectTransform>().localPosition = localPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWorldSelect) return;
        if (!isScroll)
        {
            InputRightAndLeft();
        }
        else
        {
            ScrollUpdate();
        }
    }

    /// <summary>
    /// 入力処理
    /// </summary>
    void InputRightAndLeft()
    {
        float value = 0.0f;
        if (Input.GetButton("Horizontal"))
        {
            value = Input.GetAxisRaw("Horizontal");
            if (value == -1.0f)
            {
                if (nowSelectIndex == 0) return;
                --nowSelectIndex;
                isScroll = true;
                SEPlayer.SelectPlay();
            }
            else
            {
                if (nowSelectIndex == transform.childCount - 1) return;
                ++nowSelectIndex;
                isScroll = true;
                SEPlayer.SelectPlay();
            }
        }
        if (Input.GetButton("Fire2"))
        {
            SEPlayer.EnterPlay();
            isWorldSelect = false;
            transform.GetChild(nowSelectIndex).GetChild(0).gameObject.SetActive(true);
        }
    }
    
    void ScrollUpdate()
    {
        float pointx = (float)(-nowSelectIndex) * interval;
        float lerpx = Mathf.Lerp(GetComponent<RectTransform>().localPosition.x, pointx, scrollSpeed);

        Vector3 vec = GetComponent<RectTransform>().localPosition;
        vec.x = lerpx;
        GetComponent<RectTransform>().localPosition = vec;

        if(Mathf.Abs(GetComponent<RectTransform>().localPosition.x - pointx) < 0.01f)
        {
            vec = GetComponent<RectTransform>().localPosition;
            vec.x = pointx;

            GetComponent<RectTransform>().localPosition = vec;
            isScroll = false;
        }
    }
}
