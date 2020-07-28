using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeChangeButton : BaseSelectedObject
{
	[Space, SerializeField]
	Color m_selectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	[SerializeField]
	Color m_nonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
	[SerializeField]
	UnityEngine.UI.Image m_image = null;

	VolumeChangeManager m_manager = null;
	int m_thisVolumeIndex = 0;

	public void SetThisIndex(VolumeChangeManager manager , int index)
	{
		m_manager = manager;
		m_thisVolumeIndex = index;
	}
	public void SetSelectColor(bool isSelect)
	{
		if (isSelect) m_image.color = m_selectedColor;
		else m_image.color = m_nonSelectedColor;
	}

	public override void AwakeCursor()
	{
	}

	public override void OffCursor()
	{
	}

	public override void OnCursor()
	{
		if (m_manager != null) m_manager.SelectVolumeIndex(m_thisVolumeIndex);
	}

	public override void OnEnter()
	{
	}
}
