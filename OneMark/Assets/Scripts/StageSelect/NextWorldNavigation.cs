using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWorldNavigation : MonoBehaviour
{
	static readonly int m_cToDisableTriggerID = Animator.StringToHash("ToDisable");
	static readonly int m_cToEnableTriggerID = Animator.StringToHash("ToEnable");
	static readonly int m_cStartDisableTriggerID = Animator.StringToHash("StartDisable");
	static readonly int m_cMinSelectWorldIndex = 0;
	static readonly int m_cMaxSelectWorldIndex = 3;

	bool m_isSwitchDisabled
	{
		get
		{
			int clearStages = DataManager.instance.saveData.numClearStages; if (clearStages == -1) clearStages = 0;
			if (clearStages > 0) --clearStages;

			return m_isEnabled &&
				((m_isThisPlusSelected && m_menuInput.nowSelectIndex == m_cMaxSelectWorldIndex)
				|| (!m_isThisPlusSelected && m_menuInput.nowSelectIndex == m_cMinSelectWorldIndex)
				|| (m_isThisPlusSelected && m_menuInput.nowSelectIndex + 1 == OneMarkSceneManager.cStageSceneIndexes[clearStages].x)
				|| OneMarkSceneManager.cStageSceneIndexes[clearStages].x <= 1);
		}
	}
	bool m_isSwitchEnabled
	{
		get
		{
			int clearStages = DataManager.instance.saveData.numClearStages; if (clearStages == -1) clearStages = 0;
			if (clearStages > 0) --clearStages;

			return !m_isEnabled &&
				(!(m_isThisPlusSelected && m_menuInput.nowSelectIndex == m_cMaxSelectWorldIndex)
				&& !(!m_isThisPlusSelected && m_menuInput.nowSelectIndex == m_cMinSelectWorldIndex)
				&& !(m_isThisPlusSelected && m_menuInput.nowSelectIndex + 1 == OneMarkSceneManager.cStageSceneIndexes[clearStages].x)
				&& !(OneMarkSceneManager.cStageSceneIndexes[clearStages].x <= 1));
		}
	}

	[SerializeField]
	MenuInput m_menuInput = null;
	[SerializeField]
	Animator m_animator = null;
	[SerializeField]
	bool m_isThisPlusSelected = false;

	bool m_isEnabled = true;

	void Start()
	{
		if (m_isSwitchDisabled)
		{
			m_animator.SetTrigger(m_cStartDisableTriggerID);
			m_isEnabled = false;
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (m_isSwitchDisabled)
		{
			m_animator.SetTrigger(m_cToDisableTriggerID);
			m_isEnabled = false;
		}
		else if (m_isSwitchEnabled)
		{
			m_animator.SetTrigger(m_cToEnableTriggerID);
			m_isEnabled = true;
		}
	}
}
