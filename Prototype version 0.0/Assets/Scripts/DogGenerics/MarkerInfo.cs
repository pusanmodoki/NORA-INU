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
	public MarkerInfo(LinkMarking linkMarking, float attack)
	{
		this.m_drawingAttackValue = attack;
		this.linkMarking = linkMarking;
	}

	public LinkMarking linkMarking { get; private set; } = null;
	/// <summary>marking atttack value</summary>
	public float attack { get { return m_drawingAttackValue; } }

	[SerializeField, Tooltip("drawing marking atttack value")]
	float m_drawingAttackValue = 0.0f;
}