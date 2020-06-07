using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{
    [SerializeField, Header("Input"), Tooltip("カーソル移動ボタン")]
    string m_inputMove = "Vertical";

    [SerializeField, Tooltip("決定ボタン")]
    string m_inputDirected = "Fire1";

    [SerializeField, Range(0.0f, 1.0f), Tooltip("入力のデッドゾーン")]
    float m_deadZone = 0.1f;

    [SerializeField]
    bool m_isInverse = false;

    [SerializeField, Tooltip("カーソル移動のインターバル")]
    float m_interval = 0.5f;

    [SerializeField, Header("Selected Object Infomation"), Tooltip("参照してるインデックス")]
    int m_nowSelectIndex = 0;

    [SerializeField, Tooltip("選択オブジェクトのリスト（ボタンとか）")]
    List<BaseSelectedObject> m_selectedObjects = new List<BaseSelectedObject>();

	[SerializeField]
	bool m_isStartCallOffCursor = true;

    [SerializeField, Header("Cursor Objecr")]
    BaseCursorObject m_cursorObject = null;


    [SerializeField, Header("Sound Effect")]
    AudioSource m_soundEnter = null;

    [SerializeField]
    AudioSource m_soundSelect = null;

    public int objectCount { get { return m_selectedObjects.Count; } }
    public bool isSelectInput { get; private set; } = true;
    public int nowSelectIndex { get { return m_nowSelectIndex; } }
	public bool isEnableInput { get; set; } = true;

    public List<BaseSelectedObject> selectedObjects { get { return m_selectedObjects; } }


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

        if (m_cursorObject)
        {
            m_cursorObject.SetMenu(this);
        }
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

            if (m_soundSelect)
            {
                m_soundSelect.Play();
            }
        }
    }

    void Direct()
    {
        if (m_inputDirected != null && m_inputDirected.Length > 0
			&& Input.GetButtonDown(m_inputDirected))
        {
            m_selectedObjects[m_nowSelectIndex].OnEnter();
            if (m_soundEnter)
            {
                m_soundEnter.Play();
            }
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
