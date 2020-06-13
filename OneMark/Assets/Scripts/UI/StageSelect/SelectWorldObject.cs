using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectWorldObject : BaseSelectedObject
{
	static readonly int m_cToEnableTriggerID = Animator.StringToHash("ToEnable");

	[SerializeField]
	LogoAnimation m_animation = null;
    //[SerializeField]
    //Animator m_fade = null;
    [SerializeField]
	StageSlide m_stageSlide = null;
	[SerializeField]
	SelectSoundPlayer m_soundPlayer = null;
	[SerializeField]
	MenuInput m_stageSelect = null;
	//[SerializeField]
	//Animator m_stageSelectAnimation = null;
	//[SerializeField]

	int m_disableID = -1;

	void Update()
	{
		if (!isSelected) return;

		if (m_stageSlide.isSlide && m_disableID == -1)
			menu.StartDisableEvent(out m_disableID);
		else if (!m_stageSlide.isSlide && m_disableID != -1)
		{
			menu.EndDisableEvent(m_disableID);
			m_disableID = -1;
		}
	}

	public override void OnEnter() { }

	public override void OnCursor()
	{
		m_animation.LogoOn();
        m_soundPlayer.SelectPlay();
        m_stageSelect.gameObject.SetActive(true);
		m_stageSelect.ForceSelect(0);
	//	if (m_stageSelectAnimation) m_stageSelectAnimation.SetTrigger(m_cToEnableTriggerID);

		StageSelectIndexer.index.x = menu.nowSelectIndex + 1;
		m_stageSlide.StartSlide();
        //m_fade.SetTrigger("OnFade");
    }
	
	public override void OffCursor()
	{
		if (m_stageSelect != null) m_stageSelect.gameObject.SetActive(false);
        if (m_animation != null) m_animation.LogoOff();
    }

	public override void AwakeCursor()
	{
		m_animation.LogoOn();
		StageSelectIndexer.index.x = menu.nowSelectIndex + 1;
	}
}
