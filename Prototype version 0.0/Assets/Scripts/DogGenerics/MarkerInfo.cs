using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Markerクラスからメッセージ送信する際に使用するMarkerInfo
/// </summary>
[System.Serializable]
public class MarkerInfo
{
	/// <summary>[コンストラクタ]</summary>
	public MarkerInfo(float attack)
	{
		this.m_drawingAttackValue = attack;
	}

	/// <summary>marking atttack value</summary>
	public float attack { get { return m_drawingAttackValue; } }

	[SerializeField]
	float m_drawingAttackValue = 0.0f;
}