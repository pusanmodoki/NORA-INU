using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneActiveChangeButton : BaseSelectedObject
{
	[SerializeField]
	SceneName m_changeActiveSceneName = default;
	[SerializeField]
	bool m_isSetEnterValue = false;

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
		OneMarkSceneManager.instance.SetActiveAccessoryScene(m_changeActiveSceneName, m_isSetEnterValue);
	}

	public override void AwakeCursor()
	{
		m_image.color = m_selectedColor;
	}
}
