using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更新を行わないデフォルトクラスとなるDefaultCheckPoint
/// </summary>
public class DefaultCheckPoint : BaseCheckPoint
{
	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
		if (this.isLinked)
			Debug.Log("Hit!");
		else
			Debug.Log("Not Hit!");
	}
}
