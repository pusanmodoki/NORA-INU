using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelectButton : BaseSelectedObject
{
	[SerializeField]
	Color m_selectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	[SerializeField]
	Color m_nonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
	[SerializeField]
	UnityEngine.UI.Image m_image = null;
	[SerializeField]
	MenuInput m_subMenu = null;

	int m_disableID = -1;
	
	public override void AwakeCursor()
	{
        m_image.color = m_selectedColor;

		if (m_disableID != -1)
		{
			m_subMenu.EndDisableEvent(m_disableID);
			m_disableID = -1;
		}
	}

	public override void OffCursor()
	{
		m_image.color = m_nonSelectedColor;

		if (m_disableID == -1)
			m_subMenu.StartDisableEvent(out m_disableID);
	}

	public override void OnCursor()
	{
        m_image.color = m_selectedColor;

		if (m_disableID != -1)
		{
			m_subMenu.EndDisableEvent(m_disableID);
			m_disableID = -1;
		}
	}

	public override void OnEnter()
	{
	}
}
