using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectIcon : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = "";

    [SerializeField]
    private bool isSelected = false;

    public bool m_isSelected { get { return isSelected; } set { isSelected = value; } }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }


    public void ThisSelectSceneTrans()
    {
        SceneFadeTrans.SimpleFadeTrans(nextSceneName, Color.white, 1.0f);
    }
}
