using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWorldNavigation : MonoBehaviour
{
	static readonly int m_cToDisableTriggerID = Animator.StringToHash("ToDisable");
	static readonly int m_cToEnableTriggerID = Animator.StringToHash("ToEnable");
	static readonly int m_cStartDisableTriggerID = Animator.StringToHash("StartDisable");

	[SerializeField]
	MenuInput m_menuInput = null;
	[SerializeField]
	Animator m_animator = null;
	[SerializeField]
	int m_disableIndex = 0;

	bool m_isEnabled = true;

	void Start()
	{
		if (m_isEnabled & m_menuInput.nowSelectIndex == m_disableIndex)
		{
			m_animator.SetTrigger(m_cStartDisableTriggerID);
			m_isEnabled = false;
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (m_isEnabled & m_menuInput.nowSelectIndex == m_disableIndex)
		{
			m_animator.SetTrigger(m_cToDisableTriggerID);
			m_isEnabled = false;
		}
		else if (!m_isEnabled & m_menuInput.nowSelectIndex != m_disableIndex)
		{
			m_animator.SetTrigger(m_cToEnableTriggerID);
			m_isEnabled = true;
		}
	}
}
