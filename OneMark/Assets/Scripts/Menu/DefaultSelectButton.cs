using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultSelectButton : BaseSelectedObject
{
	public enum SceneTransType
	{
		Title = 0,
		StageSelect,
		GameStart,
		NextStage,
		Restart
	}

    [SerializeField]
    SceneTransType m_state = SceneTransType.Title;

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
		switch (m_state)
		{
			case SceneTransType.Title:
				{
					OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Title);
					break;
				}
			case SceneTransType.StageSelect:
				{
					OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.StageSelect);
					break;
				}
			case SceneTransType.GameStart:
				{
					OneMarkSceneManager.instance.MoveStageScene(new Vector2Int(0, 1));
					break;
				}
			case SceneTransType.NextStage:
				{
					OneMarkSceneManager.instance.MoveNextStage();
					break;
				}
			case SceneTransType.Restart:
				{
					OneMarkSceneManager.instance.ReloadScene();
					break;
				}
		}

    }

	public override void AwakeCursor()
	{
		m_image.color = m_selectedColor;
	}
}
