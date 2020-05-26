using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSlide : MonoBehaviour
{
    [SerializeField]
    private int nowSelectIndex = 0;

    [SerializeField]
    private float interval = 40.0f;

    [SerializeField]
    private float scrollSpeed = 0.03f;

    [SerializeField]
    private bool isInput = false;

    public int worldIndex { get { return nowSelectIndex; } }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            Vector3 localPos = transform.GetChild(i).GetComponent<Transform>().localPosition;
            localPos.x = i * interval;
            transform.GetChild(i).GetComponent<Transform>().localPosition = localPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInput)
        {
            InputRightAndLeft();
        }
        
        ScrollUpdate();

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
                isInput = true;
            }
            else
            {
                if (nowSelectIndex == transform.childCount - 1) return;
                ++nowSelectIndex;
                isInput = true;
            }
        }
    }
    
    void ScrollUpdate()
    {
        float pointx = (float)(-nowSelectIndex) * interval;
        float lerpx = Mathf.Lerp(GetComponent<Transform>().localPosition.x, pointx, scrollSpeed);

        Vector3 vec = GetComponent<Transform>().localPosition;
        vec.x = lerpx;
        GetComponent<Transform>().localPosition = vec;

        if(Mathf.Abs(GetComponent<Transform>().localPosition.x - pointx) < 2.5f)
        {
            isInput = false;
        }
    }
}
