using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更新を行わないデフォルトクラスとなるDefaultMarkPoint 
/// </summary>
public class DefaultMarkPoint : BaseMarkPoint
{
	[SerializeField]
	GameObject m_uiObject = null;
	[SerializeField]
	UnityEngine.UI.Slider m_uiSlider = null;

	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
		if (isLinked)
			m_uiSlider.value = timeRemaining / effectiveSeconds;
	}

	/// <summary>
	/// [LinkPoint] (Virtual)
	/// ポイントがリンクされた際にコールバックされる関数
	/// </summary>
	public override void LinkPoint()
	{
		m_uiObject.SetActive(true);
	}
	/// <summary>
	/// [UnlinkPoint] (Virtual)
	/// ポイントがリンク解除された際にコールバックされる関数
	/// </summary>
	public override void UnlinkPoint()
	{
		m_uiObject.SetActive(false);
	}
}
