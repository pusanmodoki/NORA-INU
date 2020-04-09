using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

/// <summary>
/// 突進 & Markingを行うDogRushingAndMarking
/// </summary>
public class DogRushingAndMarking : BaseDogAIFunction
{
	/// <summary>
	/// States
	/// </summary>
	enum State
	{
		/// <summary>突進</summary>
		Rushing,
		/// <summary>回転</summary>
		Rotation,
		/// <summary>マーキング</summary>
		Marking, 
	}

	/// <summary>突進時追加加速度</summary>
	[Header("Rushing"), SerializeField, Tooltip("突進時追加加速度")]
	float m_rushingAddAcceleration = 10.0f;
	/// <summary>到着と見做す距離(半径で当たり判定を行う)</summary>
	[SerializeField, Tooltip("到着と見做す距離(半径で当たり判定を行う)")]
	float m_rushingArrivalDistance = 0.8f;

	/// <summary>マーキング時回転</summary>
	[Header("Rotation"), SerializeField, Tooltip("マーキング時回転")]
	Vector3 m_markingRotation = Vector3.zero;
	/// <summary>回転速度 (this * deltaTime)</summary>
	[SerializeField, Tooltip("回転速度 (this * deltaTime)")]
	float m_rotationSpeed = 0.9f;
	/// <summary>回転時間</summary>
	[SerializeField, Tooltip("回転時間")]
	float m_rotationSeconds = 1.0f;

	/// <summary>マーキング時間</summary>
	[Header("Marking"), SerializeField, Tooltip("マーキング時間")]
	float m_markingSeconds = 0.9f;

	BaseMarkPoint m_markPoint = null;
	Vector3 m_markPointPosition= Vector3.zero;
	LayerMaskEx m_markPointLayerMask = 0;

	Quaternion m_targetRotation = Quaternion.identity;
	State m_state;

	/// <summary>
	/// [SetAdvanceInformation]
	/// 実行に必要な事前情報を入力する
	/// 引数1: 目標ポイント
	/// 引数2: ポイントの座標
	/// </summary>
	public void SetAdvanceInformation(BaseMarkPoint markPoint, Vector3 position)
	{
		m_markPoint = markPoint;
		m_markPointPosition = position;
		m_markPointLayerMask.SetValue(markPoint.gameObject);
	}

	/// <summary>
	/// [AIBegin]
	/// 関数初回実行時に呼ばれるコールバック関数
	/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// </summary>
	public override void AIBegin(BaseAIFunction beforeFunction)
	{
		if (m_markPoint == null) return;

		//加速度設定
		dogAIAgent.speedChanger.SetManualAcceleration(m_rushingAddAcceleration);

		//初期化
		SetUpdatePosition(true);
		navMeshAgent.destination = m_markPointPosition;
		m_state = State.Rushing;
	}
	/// <summary>
	/// [AIEnd]
	/// 関数が実行登録を解除される際に呼ばれるコールバック関数
	/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// </summary>
	public override void AIEnd(BaseAIFunction nextFunction)
	{
		m_markPoint = null;
		navMeshAgent.updateRotation = true;
	}

	/// <summary>
	/// [AIUpdate]
	/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
	/// 引数1: 更新識別子
	/// </summary>
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		//万が一の無効条件
		if (m_markPoint == null)
		{
			EndAIFunction(updateIdentifier);
			return;
		}
		else if (m_markPoint.isLinked)
		{
			EndAIFunction(updateIdentifier);
			return;
		}

		//state switch
		switch (m_state)
		{
			//突進
			case State.Rushing:
				{
					//半径でOverlapを行う
					var hitCollisions = Physics.OverlapSphere(transform.position, m_rushingArrivalDistance, m_markPointLayerMask);

					//指定ポイントがヒットしているか確認
					for (int i = 0, length = hitCollisions.Length; i < length; ++i)
					{
						//指定ポイントと合致
						if (hitCollisions[i].gameObject.GetInstanceID() == m_markPoint.gameObject.GetInstanceID())
						{
							//移動, 回転停止
							navMeshAgent.updatePosition = false;
							navMeshAgent.updateRotation = false;
							dogAIAgent.speedChanger.SetManualAcceleration(0.0f);

							////回転を行う
							//m_targetRotation = Quaternion.LookRotation(
							//	(m_markPoint.transform.position - transform.position).normalized, transform.up) * Quaternion.Euler(m_markingRotation);
							//Stateを進める
							m_state = State.Rotation;
							//Timer再スタート
							timer.Start();
							break;
						}
					}

					break;
				}

			//回転
			case State.Rotation:
				{
					//指定時間経過でステートを進める
					if (timer.elapasedTime >= m_rotationSeconds)
					{
						m_state = State.Marking;
						timer.Start();
					}

					//回転
					transform.rotation =
						Quaternion.Slerp(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
					break;
				}
			//マーキング実行
			case State.Marking:
				{
					//指定時間経過
					if (timer.elapasedTime >= m_markingSeconds)
					{
						//ポイントをリンクさせる
						m_markPoint.LinkPlayer(dogAIAgent.linkPlayer, dogAIAgent);
						//待て！
						dogAIAgent.SetSitAndStay(true, m_markPoint);
						//終了
						EndAIFunction(updateIdentifier);
					}

					break;
				}
			default:
				break;
		}
	}
}
