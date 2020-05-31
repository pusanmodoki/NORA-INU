using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointCount : MonoBehaviour
{
    [SerializeField]
    private GameObject checkPointUIPrefab = null;

    [SerializeField]
    private float interval = 100.0f;


    // Start is called before the first frame update
    void Start()
    {
        foreach(var check in CheckPointManager.instance.allPoints)
        {
            GameObject uiObject = Instantiate(checkPointUIPrefab, transform);
            uiObject.GetComponent<CheckPointUI>().checkPoint = check.Value;
        }


        if(transform.childCount % 2 == 0)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                if(i % 2 == 0)
                {
                    transform.GetChild(i).GetComponent<RectTransform>().localPosition =
                        new Vector3(0.0f, (float)(i / 2) * interval + (interval / 2.0f), 0.0f);
                }
                else
                {
                    transform.GetChild(i).GetComponent<RectTransform>().localPosition =
                        new Vector3(0.0f, -((float)(i / 2) * interval + (interval / 2.0f)), 0.0f);
                }
            }
        }
        else
        {
            transform.GetChild(0).GetComponent<RectTransform>().localPosition = Vector3.zero;
            for (int i = 1; i < transform.childCount; ++i)
            {
                if (i % 2 == 0)
                {
                    transform.GetChild(i).GetComponent<RectTransform>().localPosition =
                        new Vector3(0.0f, (float)((i + 1) / 2) * interval, 0.0f);
                }
                else
                {
                    transform.GetChild(i).GetComponent<RectTransform>().localPosition =
                        new Vector3(0.0f, -((float)((i + 1) / 2) * interval), 0.0f);
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
