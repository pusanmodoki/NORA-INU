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

	[SerializeField, Tooltip("入力受付状態")]
	bool m_isEnableInput = true;

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

	Timer m_timer = new Timer();

    public int objectCount { get { return m_selectedObjects.Count; } }
    public bool isIntervalInput { get; private set; } = true;
    public int nowSelectIndex { get { return m_nowSelectIndex; } }
	public bool isEnableInput { get { return m_isEnableInput; } set { m_isEnableInput = value; } }

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
			OnInputInterval();
			m_selectedObjects[m_nowSelectIndex].isSelected = true;
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

        if (!isIntervalInput) { Select(); }
		else { CheckInputIntervalTimer(); }
        Direct();
    }


    void Select()
    {
        int prevIndex = m_nowSelectIndex;
		int inputValue = MoveInputCheck();

		AddSelectIndex(inputValue);

        if(m_nowSelectIndex != prevIndex)
        {
            m_selectedObjects[prevIndex].OffCursor();
            m_selectedObjects[prevIndex].isSelected = false;

            m_selectedObjects[m_nowSelectIndex].OnCursor();
			OnInputInterval();
            m_selectedObjects[m_nowSelectIndex].isSelected = true;

            if (m_soundSelect) { m_soundSelect.Play(); }

			if (m_cursorObject)
			{
				m_cursorObject.SelectUpdate(inputValue);
				if ((nowSelectIndex == 0 && m_isInverse) ||
					(nowSelectIndex == objectCount - 1 && !m_isInverse)) 
				{
					m_cursorObject.SelectTopIndex();
				}
				else if ((nowSelectIndex == 0 && !m_isInverse) ||
					(nowSelectIndex == objectCount - 1 && m_isInverse))
				{
					m_cursorObject.SelectBottomIndex();
				}
			}
		}
    }

    void Direct()
    {
        if (m_inputDirected != null && m_inputDirected.Length > 0
			&& Input.GetButtonDown(m_inputDirected))
        {
            m_selectedObjects[m_nowSelectIndex].OnEnter();
            if (m_soundEnter) { m_soundEnter.Play(); }
			if (m_cursorObject) { m_cursorObject.OnEnter(); }
        }
    }

    int MoveInputCheck()
    {
        float inputValue = Input.GetAxis(m_inputMove);

        if ((inputValue > m_deadZone && !m_isInverse) ||
            (inputValue < -m_deadZone && m_isInverse))
        {
            return 1;
		}
		else if ((inputValue > m_deadZone && m_isInverse) ||
            (inputValue < -m_deadZone && !m_isInverse))
        {
			return -1;
		}
		return 0;
    }

	void AddSelectIndex(int _add)
	{
		m_nowSelectIndex += _add;
		if (m_nowSelectIndex < 0)
		{
			m_nowSelectIndex = 0;
		}
		else if (m_nowSelectIndex >= objectCount)
		{
			m_nowSelectIndex = objectCount - 1;
		}
	}
    

    void SetActiveInput(int _isInput)
    {
		if(_isInput == 0) { isEnableInput = false; }
		else { isEnableInput = true; }
    }

	void OnInputInterval()
	{
		isIntervalInput = true;
		m_timer.Start();
	}

	void CheckInputIntervalTimer()
	{
		if(m_timer.elapasedTime > m_interval)
		{
			isIntervalInput = false;
			m_timer.Stop();
		}
	}
}
