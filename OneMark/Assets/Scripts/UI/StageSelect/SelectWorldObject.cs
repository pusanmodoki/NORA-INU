using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectWorldObject : BaseSelectedObject
{
	[SerializeField]
	LogoAnimation m_animation = null;
	[SerializeField]
	StageSlide m_stageSlide = null;
	[SerializeField]
	MenuInput m_stageSelect = null;
	[SerializeField]
	SelectSoundPlayer m_soundPlayer = null;

	void Update()
	{
		if (isSelected)
			menu.isEnableInput = !m_stageSlide.isSlide;
	}

	public override void OnEnter() { }

	public override void OnCursor()
	{
		m_animation.LogoOn();
		m_soundPlayer.SelectPlay();
		m_stageSelect.ForceSelect(0);
		StageSelectIndexer.index.x = menu.nowSelectIndex + 1;
		m_stageSlide.StartSlide();
	}

	public override void OffCursor()
	{
		m_animation.LogoOff();
	}

	public override void AwakeCursor()
	{
		m_animation.LogoOn();
		StageSelectIndexer.index.x = menu.nowSelectIndex + 1;
	}
}
