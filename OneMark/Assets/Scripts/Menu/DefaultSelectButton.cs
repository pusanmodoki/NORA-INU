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
		Restart,
		Return,
		QuitApplication
	}

    [SerializeField]
    SceneTransType m_state = SceneTransType.Title;

	[SerializeField]
	TriggerEvent m_triggerEvent = null;

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
					if (m_triggerEvent != null) m_triggerEvent.OnTrigger(TriggerEvent.cDefaultEnable);
					OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Title);
					break;
				}
			case SceneTransType.StageSelect:
				{
					if (m_triggerEvent != null) m_triggerEvent.OnTrigger(TriggerEvent.cDefaultEnable);
					OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.StageSelect);
					break;
				}
			case SceneTransType.GameStart:
				{
					if (m_triggerEvent != null) m_triggerEvent.OnTrigger(TriggerEvent.cDefaultEnable);
					OneMarkSceneManager.instance.MoveStageScene(new Vector2Int(0, 1));
					break;
				}
			case SceneTransType.NextStage:
				{
					if (m_triggerEvent != null) m_triggerEvent.OnTrigger(TriggerEvent.cDefaultEnable);
					OneMarkSceneManager.instance.MoveNextStage();
					break;
				}
			case SceneTransType.Restart:
				{
					if (m_triggerEvent != null) m_triggerEvent.OnTrigger(TriggerEvent.cDefaultEnable);
					OneMarkSceneManager.instance.ReloadScene();
					break;
				}
			case SceneTransType.Return:
				{
					if (m_triggerEvent != null) m_triggerEvent.OnTrigger(TriggerEvent.cDefaultEnable);
					OneMarkSceneManager.instance.SetActiveAccessoryScene(
						gameObject.scene.name, false);
					break;
				}
			case SceneTransType.QuitApplication:
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
				 UnityEngine.Application.Quit();
#endif
				break;
		}

    }

	public override void AwakeCursor()
	{
		m_image.color = m_selectedColor;
	}
}
