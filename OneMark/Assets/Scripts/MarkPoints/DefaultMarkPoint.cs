using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更新を行わないデフォルトクラスとなるDefaultMarkPoint 
/// </summary>
public class DefaultMarkPoint : BaseMarkPoint
{
	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
    }

    /// <summary>
    /// [LinkPoint] (Virtual)
    /// ポイントがリンクされた際にコールバックされる関数
    /// </summary>
    public override void LinkPoint()
	{
	}
	/// <summary>
	/// [UnlinkPoint] (Virtual)
	/// ポイントがリンク解除された際にコールバックされる関数
	/// </summary>
	public override void UnlinkPoint()
	{
	}
}
