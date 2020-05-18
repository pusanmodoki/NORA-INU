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
	public enum State
	{
		/// <summary>Null</summary>
		Null,
		/// <summary>突進</summary>
		Rushing,
		/// <summary>回転</summary>
		Rotation,
		/// <summary>マーキング</summary>
		Marking,
		/// <summary>終了</summary>
		End,
	}

	/// <summary>State</summary>
	public State functionState { get; private set; } = State.Null;

	/// <summary>This AnimationController</summary>
	[Header("Animation & Effect & SE"), SerializeField, Tooltip("This AnimationController")]
	DogAnimationController m_animationController = null;
	/// <summary>Marking effect object</summary>
	[SerializeField, Tooltip("Marking effect object")]
	GameObject m_markingEffect = null;
	/// <summary>This SEPlayer</summary>
	[SerializeField, Tooltip("This SEPlayer")]
	SEPlayer m_sePlayer = null;
	/// <summary>Marking se index</summary>
	[SerializeField, Tooltip("Marking se index")]
	int m_markingSEIndex = 0;
	
	/// <summary>突進時追加加速度</summary>
	[Header("Rushing"), SerializeField, Tooltip("突進時追加加速度")]
	float m_rushingAddAcceleration = 10.0f;
	/// <summary>到着と見做す距離(半径で当たり判定を行う)</summary>
	[SerializeField, Tooltip("到着と見做す距離(半径で当たり判定を行う)")]
	float m_rushingArrivalDistance = 0.8f;

	/// <summary>回転速度 (this * deltaTime)</summary>
	[Header("Rotation"), SerializeField, Tooltip("回転速度 (this * deltaTime)")]
	float m_rotationSpeed = 0.9f;
	/// <summary>回転時間</summary>
	[SerializeField, Tooltip("回転時間")]
	float m_rotationSeconds = 1.0f;

	/// <summary>マーキング時間</summary>
	[Header("Marking"), SerializeField, Tooltip("マーキング時間")]
	float m_markingSeconds = 0.9f;

	BaseMarkPoint m_markPoint = null;
	LayerMaskEx m_markPointLayerMask = 0;

	Quaternion m_targetRotation = Quaternion.identity;

	/// <summary>
	/// [SetAdvanceInformation]
	/// 実行に必要な事前情報を入力する
	/// 引数1: 目標ポイント
	/// </summary>
	public void SetAdvanceInformation(BaseMarkPoint markPoint)
	{
		m_markPoint = markPoint;
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
		navMeshAgent.destination = m_markPoint.transform.position;
		functionState = State.Rushing;
		//Animation Set
		m_animationController.editAnimation.SetTriggerRolling();

		////強制的に増加させる
		//if (m_markPoint.linkPlayerID == dogAIAgent.linkPlayer.GetInstanceID())
		//	m_markPoint.SetForceAscendingEffective(true);
	}
	/// <summary>
	/// [AIEnd]
	/// 関数が実行登録を解除される際に呼ばれるコールバック関数
	/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// </summary>
	public override void AIEnd(BaseAIFunction nextFunction)
	{
		if (functionState != State.End)
		{
			m_animationController.editAnimation.SetTriggerForceChangeStand();
			PlayerAndTerritoryManager.instance.allPlayers[dogAIAgent.linkPlayer.GetInstanceID()].input.ChangeShotFlags(dogAIAgent, false);
		}

		if (functionState == State.Marking)
		{
			m_sePlayer.Stop(m_markingSEIndex);
			m_markingEffect.SetActive(false);
		}

		if (functionState != State.End && functionState != State.Rushing
			&& m_markPoint.linkServantID == dogAIAgent.aiAgentInstanceID)
		{
#if UNITY_EDITOR
			Debug.Log("Error!! DogRushingAndMarking: AIEnd->(functionState != State.End && functionState != State.Rushing \n" +
				"m_markPoint.linkServantID == dogAIAgent.aiAgentInstanceID");
#endif
			//ポイントをリンク解除
			m_markPoint.UnlinkPlayer();
			//動け！
			dogAIAgent.SetSitAndStay(false, m_markPoint);
		}

		m_markPoint = null;
		navMeshAgent.isStopped = false;
		functionState = State.Null;
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
#if UNITY_EDITOR
			Debug.Log("Error!! DogRushingAndMarking: AIUpdate->m_markPoint == null");
#endif
			EndAIFunction(updateIdentifier);
			return;
		}
		else if (m_markPoint.isLinked && 
			(m_markPoint.linkServantID != dogAIAgent.aiAgentInstanceID && m_markPoint.linkServantID != -1))
		{
#if UNITY_EDITOR
			Debug.Log("Error!! DogRushingAndMarking: AIUpdate->m_markPoint.isLinked && \n" + 
				"(m_markPoint.linkServantID != dogAIAgent.aiAgentInstanceID && m_markPoint.linkServantID != -1)");
#endif
			EndAIFunction(updateIdentifier);
			return;
		}

		//state switch
		switch (functionState)
		{
			//突進
			case State.Rushing:
				{
					//設定
					navMeshAgent.destination = m_markPoint.transform.position;

					//半径でOverlapを行う
					var hitCollisions = Physics.OverlapSphere(transform.position, m_rushingArrivalDistance, m_markPointLayerMask);

					//指定ポイントがヒットしているか確認
					for (int i = 0, length = hitCollisions.Length; i < length; ++i)
					{
						//指定ポイントと合致
						if (hitCollisions[i].gameObject.GetInstanceID() == m_markPoint.gameObject.GetInstanceID())
						{
							//強制的に増加させる
							if (m_markPoint.linkPlayerID != dogAIAgent.linkPlayer.GetInstanceID())
								m_markPoint.AddFirstLinkBonus();
							//ポイントをリンクさせる
							m_markPoint.LinkPlayer(dogAIAgent.linkPlayer, dogAIAgent);
							//待て！
							dogAIAgent.SetSitAndStay(true, m_markPoint);

							//移動, 回転停止
							navMeshAgent.isStopped = true;
							dogAIAgent.speedChanger.SetManualAcceleration(0.0f);

							//回転を決める
							Vector3 absolute = m_markPoint.transform.position - transform.position;
							absolute.y = 0.0f;
							m_targetRotation = Quaternion.LookRotation(absolute.normalized) * Quaternion.AngleAxis(-90, Vector3.up);
							//Stateを進める
							functionState = State.Rotation;
							//Animation Set
							m_animationController.editAnimation.SetTriggerMarking();
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
						m_sePlayer.PlaySE(m_markingSEIndex, true);
						m_markingEffect.SetActive(true);
						functionState = State.Marking;
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
						functionState = State.End;
						m_markingEffect.SetActive(false);
						m_sePlayer.Stop(m_markingSEIndex);
						//Animation Set
						m_animationController.editAnimation.SetTriggerSleepStart();
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
