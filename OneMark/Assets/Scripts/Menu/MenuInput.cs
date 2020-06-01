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
    List<BaseSelectedObject> m_selectedObjects = new List<BaseSelectedObject>();

    [SerializeField, Range(0.0f, 1.0f), Tooltip("入力のデッドゾーン")]
    float m_deadZone = 0.1f;

    [SerializeField, Tooltip("カーソル移動のインターバル")]
    float m_interval = 0.5f;

    [SerializeField]
    bool m_isInverse = false;
	[SerializeField]
	bool m_isStartCallOffCursor = true;

	public int objectCount { get { return m_selectedObjects.Count; } }
    public bool isSelectInput { get; private set; } = true;
    public int nowSelectIndex { get { return m_nowSelectIndex; } }
	public bool isEnableInput { get; set; } = true;

	public void ForceSelect(int index)
	{
		if (index >= m_selectedObjects.Count) return;

		if (m_nowSelectIndex != index)
		{
			m_selectedObjects[m_nowSelectIndex].OffCursor();
			m_selectedObjects[m_nowSelectIndex].isSelected = false;
			m_nowSelectIndex = index;

			m_selectedObjects[m_nowSelectIndex].OnCursor();
			isSelectInput = false;
			m_selectedObjects[m_nowSelectIndex].isSelected = true;
			StartCoroutine("InputInterval");
		}
	}

    private void Start()
    {
        foreach(var obj in m_selectedObjects)
        {
			if (m_isStartCallOffCursor) obj.OffCursor();
            obj.SetMenu(this);
        }
        m_selectedObjects[nowSelectIndex].AwakeCursor();
    }


    // Update is called once per frame
    void Update()
    {
		if (!isEnableInput) return;

        if (isSelectInput) { Select(); }
        Direct();
    }


    void Select()
    {
        int prevIndex = m_nowSelectIndex;
		m_nowSelectIndex = MoveInputCheck();

        if(m_nowSelectIndex != prevIndex)
        {
            m_selectedObjects[prevIndex].OffCursor();
            m_selectedObjects[prevIndex].isSelected = false;

            m_selectedObjects[m_nowSelectIndex].OnCursor();
            isSelectInput = false;
            m_selectedObjects[m_nowSelectIndex].isSelected = true;
            StartCoroutine("InputInterval");
        }
    }

    void Direct()
    {
        if (m_inputDirected != null && m_inputDirected.Length > 0
			&& Input.GetButtonDown(m_inputDirected))
        {
            m_selectedObjects[m_nowSelectIndex].OnEnter();
        }
    }

    int MoveInputCheck()
    {
        float inputValue = Input.GetAxis(m_inputMove);

        if ((inputValue > m_deadZone && !m_isInverse) ||
            (inputValue < -m_deadZone && m_isInverse))
        {
            m_nowSelectIndex += 1;

			return m_nowSelectIndex >= objectCount ? objectCount - 1 : m_nowSelectIndex;
		}
		else if ((inputValue > m_deadZone && m_isInverse) ||
            (inputValue < -m_deadZone && !m_isInverse))
        {
            m_nowSelectIndex  -= 1;

			return m_nowSelectIndex < 0 ? 0 : m_nowSelectIndex;
		}
        return m_nowSelectIndex;
    }
    
    IEnumerator InputInterval()
    {
        yield return new WaitForSeconds(m_interval);
        isSelectInput = true;
    }
}
