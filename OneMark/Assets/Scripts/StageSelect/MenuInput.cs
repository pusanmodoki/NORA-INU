using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{
    [SerializeField, Tooltip("カーソル移動ボタン")]
    string m_inputMove = "Vertical";

    [SerializeField, Tooltip("決定ボタン")]
    string m_inputDirected = "Fire1";

    [SerializeField, Tooltip("参照してるインデックス")]
    int m_nowSelectIndex = 0;

    [SerializeField, Tooltip("選択オブジェクトのリスト（ボタンとか）")]
    List<SelectedObject> m_selectedObjects = new List<SelectedObject>();

    [SerializeField, Range(0.0f, 1.0f), Tooltip("入力のデッドゾーン")]
    float m_deadZone = 0.1f;

    [SerializeField, Tooltip("カーソル移動のインターバル")]
    float m_interval = 0.5f;

    public int objectCount { get { return m_selectedObjects.Count; } }

    public bool isSelectInput { get; private set; }

    private void Start()
    {
        foreach(var obj in m_selectedObjects)
        {
            obj.OffCirsol();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isSelectInput) { Select(); }
        Direct();
    }


    void Select()
    {
        int prevIndex = m_nowSelectIndex;
        m_nowSelectIndex += MoveInputCheck();
        if(m_nowSelectIndex != prevIndex)
        {
            m_selectedObjects[prevIndex].OffCirsol();
            m_selectedObjects[prevIndex].isSelected = false;
            if (m_nowSelectIndex < 0) { m_nowSelectIndex = 0; }
            else if (m_nowSelectIndex >= objectCount) { m_nowSelectIndex = objectCount - 1; }
            m_selectedObjects[m_nowSelectIndex].OnCirsol();
            isSelectInput = false;
            m_selectedObjects[m_nowSelectIndex].isSelected = true;
            StartCoroutine("InputInterval");
        }
    }

    void Direct()
    {
        if (Input.GetButtonDown(m_inputDirected))
        {
            m_selectedObjects[m_nowSelectIndex].OnEnter();
        }
    }

    int MoveInputCheck()
    {
        float inputValue = Input.GetAxis(m_inputMove);

        if(inputValue > m_deadZone)
        {
            return 1;
        }
        else if(inputValue < -m_deadZone)
        {
            return -1;
        }
        return 0;
    }
    
    IEnumerator InputInterval()
    {
        yield return new WaitForSeconds(m_interval);
        isSelectInput = true;
    }
}
