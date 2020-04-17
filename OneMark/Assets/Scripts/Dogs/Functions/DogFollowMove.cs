using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

/// <summary>
/// Playerに向けて移動するDogFollowMove
/// </summary>
public class DogFollowMove : BaseDogAIFunction
{
	/// <summary>移動時間</summary>
	[SerializeField, Tooltip("移動時間")]
	float m_moveSeconds = 0.5f;
	/// <summary>目標地点を後方にずらすための値 (-forward * this)</summary>
	[SerializeField, Tooltip("目標地点を後方にずらすための値 (-forward * this)")]
	float m_destinationBackAdjust = 1.5f;
	/// <summary>到着したとみなす距離</summary>
	[SerializeField, Tooltip("到着したとみなす距離")]
	float m_arrivalDistance = 1.0f;
	/// <summary>待機時の回転速度</summary>
	[SerializeField, Tooltip("待機時の回転速度 (Sleap)")]
	float m_rotationSpeed = 2.0f;
	/// <summary>回転済みしたとみなす角度</summary>
	[SerializeField, Tooltip("回転済みしたとみなす角度")]
	float m_arrivalRotation = 5.0f;

	Transform m_followTransform = null;

	/// <summary>
	/// [AIBegin]
	/// 関数初回実行時に呼ばれるコールバック関数
	/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// 引数2: 並列関数として実行？
	/// </summary>
	public override void AIBegin(BaseAIFunction beforeFunction)
	{
		if (dogAIAgent.isLinkPlayer && dogAIAgent.linkPlayerServantsOwnIndex >= 0)
		{
			navMeshAgent.updatePosition = true;

			m_followTransform = dogAIAgent.linkPlayer.transform;
			int playerID = dogAIAgent.linkPlayer.GetInstanceID();
			int followIndex = 0;

			for (int i = 0, count = dogAIAgent.linkPlayerServantsOwnIndex; i < count; ++i)
			{
				var servant = ServantManager.instance.servantByPlayers[playerID][i];

				if (servant.isAccompanyingPlayer)
					++followIndex;
			}

			m_followTransform = PlayerAndTerritoryManager.instance.allPlayers
				[dogAIAgent.linkPlayer.GetInstanceID()].playerInfo.followPoints[followIndex].transform;
		}


		Vector3 setDestination = m_followTransform.position;
		setDestination.y = transform.position.y;

		navMeshAgent.destination = setDestination;
	}
	/// <summary>
	/// [AIEnd]
	/// 関数が実行登録を解除される際に呼ばれるコールバック関数
	/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// 引数2: 並列関数として実行？
	/// </summary>
	public override void AIEnd(BaseAIFunction nextFunction)
	{
		navMeshAgent.isStopped = false;
	}

	/// <summary>
	/// [AIUpdate]
	/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
	/// 引数1: 更新識別子
	/// </summary>
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		if ((m_followTransform.position - transform.position).sqrMagnitude < m_arrivalDistance * m_arrivalDistance)
			navMeshAgent.isStopped = true;
		else
			navMeshAgent.isStopped = false;

		Vector3 absoluteNotYNormalized = (dogAIAgent.linkPlayer.transform.position - transform.position);
		absoluteNotYNormalized.y = 0.0f;
		if (absoluteNotYNormalized.sqrMagnitude >= 1.0f)
		{
			absoluteNotYNormalized = absoluteNotYNormalized.normalized;

			if (navMeshAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathPartial
				&& Vector3.Angle(absoluteNotYNormalized, transform.forward) > m_arrivalRotation)
			{
				Quaternion lookRotation = Quaternion.LookRotation(absoluteNotYNormalized, transform.up);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_rotationSpeed * Time.deltaTime);
			}
		}

		if (timer.elapasedTime >= m_moveSeconds)
			EndAIFunction(updateIdentifier);
	}
}
