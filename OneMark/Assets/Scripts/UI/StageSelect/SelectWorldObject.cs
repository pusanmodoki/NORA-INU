﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectWorldObject : BaseSelectedObject
{
	[SerializeField]
	LogoAnimation m_animation = null;
    //[SerializeField]
    //Animator m_fade = null;
    [SerializeField]
	StageSlide m_stageSlide = null;
	[SerializeField]
	MenuInput m_stageSelect = null;
	[SerializeField]
	SelectSoundPlayer m_soundPlayer = null;
	//[SerializeField]

	int m_disableID = -1;

	void Update()
	{
		if (!isSelected) return;

		if (m_stageSlide.isSlide && m_disableID == -1)
			menu.StartDisableEvent(out m_disableID);
		else if (!m_stageSlide.isSlide && m_disableID != -1)
			menu.EndDisableEvent(m_disableID);
	}

	public override void OnEnter() { }

	public override void OnCursor()
	{
		m_animation.LogoOn();
        m_soundPlayer.SelectPlay();
        m_stageSelect.gameObject.SetActive(true);
		m_stageSelect.ForceSelect(0);
		StageSelectIndexer.index.x = menu.nowSelectIndex + 1;
		m_stageSlide.StartSlide();
        //m_fade.SetTrigger("OnFade");
    }

    public override void OffCursor()
	{
        m_stageSelect.gameObject.SetActive(false);
        m_animation.LogoOff();

    }

	public override void AwakeCursor()
	{
		m_animation.LogoOn();
		StageSelectIndexer.index.x = menu.nowSelectIndex + 1;
	}
}
