using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            Vector3 localPos = Vector3.zero;
            localPos.x = i * interval;
            transform.GetChild(i).localPosition = localPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetButtonDown("Horizontal"))
        {
            value = Input.GetAxisRaw("Horizontal");
            if (value == -1)
            {
                if (nowSelectIndex == 0) return;
                --nowSelectIndex;
                isScroll = true;
            }
            else
            {
                if (nowSelectIndex == transform.childCount) return;
                ++nowSelectIndex;
                isScroll = true;
            }
        }
    }
    
    void ScrollUpdate()
    {
        float goalPoint = (float)nowSelectIndex * interval;

        
    }
}
