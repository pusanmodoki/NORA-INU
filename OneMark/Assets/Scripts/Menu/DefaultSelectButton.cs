using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultSelectButton : BaseSelectedObject
{
    [SerializeField]
    OneMarkSceneManager.SceneState m_state = OneMarkSceneManager.SceneState.Title;

    [SerializeField]
    Vector2Int stageNum = new Vector2Int(0, 1);

    [SerializeField]
    Color m_selectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    [SerializeField]
    Color m_nonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);

    [SerializeField]
    UnityEngine.UI.Image m_image = null;

    public override void OnCursor()
    {
        m_image.color = m_selectedColor;
    }

    public override void OffCursor()
    {
        m_image.color = m_nonSelectedColor;
    }

    public override void OnEnter()
    {
        if(m_state != OneMarkSceneManager.SceneState.Stage)
        {
            OneMarkSceneManager.instance.MoveScene(m_state);
        }
        else
        {
            OneMarkSceneManager.instance.MoveStageScene(stageNum);
        }
    }

	public override void AwakeCursor()
	{
		m_image.color = m_selectedColor;
	}
}
