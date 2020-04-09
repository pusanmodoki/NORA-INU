//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LayerMaskを拡張したLayerMaskEx class
/// </summary>
[System.Serializable]
public struct LayerMaskEx
{
    /// <summary>
    /// LayerMask getter
    /// </summary>
    public LayerMask layerMask { get { return m_layerMask; } private set { m_layerMask = value; } }
    /// <summary>
    /// Layer Bit Mask Getter
    /// </summary>
    public int value { get { return layerMask.value; } }

    [SerializeField, Tooltip("LayerMask")]
    LayerMask m_layerMask;

    /// <summary>
    /// [コンストラクタ]
    /// 引数1: レイヤーマスク
    /// </summary>
    public LayerMaskEx(int layerMask)
    {
        m_layerMask = layerMask;
    }
    /// <summary>
    /// [コンストラクタ]
    /// 引数1: レイヤーマスク
    /// </summary>
    public LayerMaskEx(LayerMask layerMask)
    {
        m_layerMask = layerMask;
	}
	/// <summary>
	/// [コンストラクタ]
	/// 引数1: GameObject
	/// </summary>
	public LayerMaskEx(GameObject gameObject)
	{
		m_layerMask = 1 << gameObject.layer;
	}

	/// <summary>
	/// [SetValue]
	/// 引数1: レイヤーマスク
	/// </summary>
	public void SetValue(int layerMask)
    {
        m_layerMask = layerMask;
    }
    /// <summary>
    /// [SetValue]
    /// 引数1: レイヤーマスク
    /// </summary>
    public void SetValue(LayerMask layer)
    {
        m_layerMask = layerMask;
	}
	/// <summary>
	/// [SetValue]
	/// 引数1: GameObject
	/// </summary>
	public void SetValue(GameObject gameObject)
	{
		m_layerMask = 1 << gameObject.layer;
	}

	/// <summary>
	/// [EqualBitsForGameObject]
	/// return: equal bits = true
	/// 引数1: GameObject
	/// </summary>
	public bool EqualBitsForGameObject(GameObject gameObject)
    {
        return (value & 1 << gameObject.layer) != 0;
    }

    /// <summary>
    /// operator to int
    /// </summary>
    public static implicit operator int(LayerMaskEx mask)
    {
        return mask.value;
    }
    /// <summary>
    /// operator to LayerMaskEx
    /// </summary>
    public static implicit operator LayerMaskEx(int mask)
    {
        return new LayerMaskEx(mask);
    }
    /// <summary>
    /// operator to LayerMask
    /// </summary>
    public static implicit operator LayerMask(LayerMaskEx mask)
    {
        return mask.layerMask;
    }
    /// <summary>
    /// operator to LayerMaskEx
    /// </summary>
    public static implicit operator LayerMaskEx(LayerMask mask)
    {
        return new LayerMaskEx(mask);
    }
}
