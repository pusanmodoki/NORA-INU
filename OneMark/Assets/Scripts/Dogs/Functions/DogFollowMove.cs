using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

/// <summary>
/// Playerに向けて移動するDogFollowMove
/// </summary>
public class DogFollowMove : BaseDogAIFunction
{
	[SerializeField, Tooltip("移動時間")]
	float m_moveSeconds = 0.5f;

	/// <summary>
	/// [AIBegin]
	/// 関数初回実行時に呼ばれるコールバック関数
	/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// 引数2: 並列関数として実行？
	/// </summary>
	public override void AIBegin(BaseAIFunction beforeFunction)
	{
		if (dogAIAgent.isLinkPlayer)
		{
			SetUpdatePosition(true);
			navMeshAgent.destination = dogAIAgent.linkPlayer.transform.position;
		}
	}
	/// <summary>
	/// [AIEnd]
	/// 関数が実行登録を解除される際に呼ばれるコールバック関数
	/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// 引数2: 並列関数として実行？
	/// </summary>
	public override void AIEnd(BaseAIFunction nextFunction)
	{
	}

	/// <summary>
	/// [AIUpdate]
	/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
	/// 引数1: 更新識別子
	/// </summary>
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		if (timer.elapasedTime >= m_moveSeconds)
			EndAIFunction(updateIdentifier);
	}
}
