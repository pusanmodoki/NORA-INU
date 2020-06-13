using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageButton : BaseSelectedObject
{
	[SerializeField]
	Image m_image = null;
	[SerializeField]
	StageSlide m_stageSlide = null;
	//[SerializeField]
	//SelectSoundPlayer m_soundPlayer = null;

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

	public override void OnEnter()
	{
		//m_soundPlayer.EnterPlay();
		OneMarkSceneManager.instance.MoveStageScene(StageSelectIndexer.index);
	}

	public override void OnCursor()
	{
		//m_soundPlayer.SelectPlay();
		m_image.color = Color.white;
		StageSelectIndexer.index.y = menu.nowSelectIndex + 1;
	}

	public override void OffCursor() { m_image.color = Color.gray; }

	public override void AwakeCursor()
	{
		m_image.color = Color.white;
		StageSelectIndexer.index.y = menu.nowSelectIndex + 1;
	}
}
