using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSelectButton : BaseSelectedObject
{
    [SerializeField]
    SpriteRenderer m_renderer = null;
    [SerializeField]
    StageSlide m_stageSlide = null;
    [SerializeField]
    SelectSoundPlayer m_soundPlayer = null;

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
        m_soundPlayer.EnterPlay();
        OneMarkSceneManager.instance.MoveStageScene(StageSelectIndexer.index);
    }

    public override void OnCursor()
    {
        m_soundPlayer.SelectPlay();
        m_renderer.color = Color.white;
        StageSelectIndexer.index.y = menu.nowSelectIndex + 1;
    }

    public override void OffCursor() { m_renderer.color = Color.gray; }

    public override void AwakeCursor()
    {
        m_renderer.color = Color.white;
        StageSelectIndexer.index.y = menu.nowSelectIndex + 1;
    }
}
