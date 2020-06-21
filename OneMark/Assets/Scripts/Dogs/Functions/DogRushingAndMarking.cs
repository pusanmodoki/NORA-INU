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
		/// <summary>マーキング終了</summary>
		MarkingEnd,
		/// <summary>終了</summary>
		FunctionEnd,
	}

	static readonly Quaternion m_targetRotation = Quaternion.LookRotation(-Vector3.forward);
	static int m_bgmChangeAgentID = -1;
	static readonly float m_cRemainingCheckDistance = 0.1f * 0.1f;
	static readonly float m_cIfPointMoveRemainingCheckDistance = 0.4f * 0.4f;

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

	/// <summary>回転速度 (this * deltaTime)</summary>
	[Header("Rotation"), SerializeField, Tooltip("回転速度 (this * deltaTime)")]
	float m_rotationSpeed = 0.9f;

	/// <summary>マーキング時間</summary>
	[Header("Marking"), SerializeField, Tooltip("マーキング時間")]
	float m_markingSeconds = 0.9f;

	BaseMarkPoint m_markPoint = null;
	Vector3 m_moveTarget = Vector3.zero;

	/// <summary>
	/// [SetAdvanceInformation]
	/// 実行に必要な事前情報を入力する
	/// 引数1: 目標ポイント
	/// </summary>
	public void SetAdvanceInformation(BaseMarkPoint markPoint)
	{
		m_markPoint = markPoint;
	}
	/// <summary>
	/// [MoveStateFromMarkingEndToFunctionEnd]
	/// functionState == MarkingEnd時, FunctionEndへStateを進める
	/// </summary>
	public void MoveStateFromMarkingEndToFunctionEnd()
	{
		if (functionState == State.MarkingEnd) functionState = State.FunctionEnd;
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
		UnityEngine.AI.NavMeshHit navMeshHit;
		UnityEngine.AI.NavMesh.SamplePosition(m_markPoint.transform.position,
			out navMeshHit, 2.0f, UnityEngine.AI.NavMesh.AllAreas);
		//m_targetPoint = navMeshHit.position;
		m_moveTarget = m_markPoint.markingTarget;
	//	m_rotationTarget = m_markPoint.transform.position;
	//	m_rotationTarget.y = 0.0f;

		SetUpdatePosition(true);
		navMeshAgent.destination = m_moveTarget;
		functionState = State.Rushing;
		//Animation Set
		m_animationController.editAnimation.state = DogAnimationController.AnimationState.Rolling;

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
		if (functionState != State.FunctionEnd)
		{
			m_animationController.editAnimation.TriggerForceChangeStand();
			//PlayerAndTerritoryManager.instance.allPlayers[dogAIAgent.linkPlayer.GetInstanceID()].input.ChangeShotFlags(dogAIAgent, false);
		}

		if (functionState == State.Marking)
		{
			m_sePlayer.Stop(m_markingSEIndex);
			m_markingEffect.SetActive(false);

			if (!MainGameManager.instance.isGameEnd
				&& m_bgmChangeAgentID == dogAIAgent.aiAgentInstanceID)
			{
				m_bgmChangeAgentID = -1;
				AudioManager.instance.FadeinBgm("Marking");
			}
			else
			{
#if UNITY_EDITOR
				Debug.Log("DogRushingAndMarking->AIUpdate: if (!MainGameManager.instance.isGameEnd)" +
					"\n && m_bgmChangeAgentID == dogAIAgent.aiAgentInstanceID");
#endif
			}
		}

		if (functionState != State.FunctionEnd && functionState != State.Rushing
			&& m_markPoint.linkServantID == dogAIAgent.aiAgentInstanceID)
		{
#if UNITY_EDITOR
			Debug.Log("Error!! DogRushingAndMarking: AIEnd->(functionState != State.End && functionState != State.Rushing \n" +
				"m_markPoint.linkServantID == dogAIAgent.aiAgentInstanceID");
#endif
			//ポイントをリンク解除
			m_markPoint.UnlinkPlayer();
			//動け！
			dogAIAgent.SetWaitAndRun(false, m_markPoint);
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
					m_moveTarget = m_markPoint.calculateMarknigTarget;
					navMeshAgent.destination = m_moveTarget;

					Vector3 position = navMeshAgent.transform.position; position.y = m_moveTarget.y;
					if ((position - m_moveTarget).sqrMagnitude 
						< (!m_markPoint.isMove ? m_cRemainingCheckDistance : m_cIfPointMoveRemainingCheckDistance))
					{
						//移動, 回転停止
						navMeshAgent.isStopped = true;
						dogAIAgent.speedChanger.SetManualAcceleration(0.0f);

						//Animation Set
						m_animationController.editAnimation.state = DogAnimationController.AnimationState.Run;

						//Stateを進める
						functionState = State.Rotation;
					}

					break;
				}

			//回転
			case State.Rotation:
				{
					Quaternion rotation = transform.rotation;

					//等値比較でステートを進める
					if (Mathf.Abs(Quaternion.Dot(rotation, m_targetRotation)) > 0.99999f)
					{
						//Animation Set
						m_animationController.editAnimation.TriggerMarking();

						//強制的に増加させる
						if (m_markPoint.linkPlayerID != dogAIAgent.linkPlayer.GetInstanceID())
							m_markPoint.AddFirstLinkBonus();
						//待て！
						dogAIAgent.SetWaitAndRun(true, m_markPoint);

						//ポイントをリンクさせる
						m_markPoint.LinkPlayer(dogAIAgent.linkPlayer, dogAIAgent);
						m_markPoint.SetCompleteMarking(true);
						m_markPoint.LinkMarkingStart();

						if (!MainGameManager.instance.isGameEnd)
						{
							m_sePlayer.PlaySE(m_markingSEIndex, true);
							m_bgmChangeAgentID = dogAIAgent.aiAgentInstanceID;
							AudioManager.instance.FadeoutBgm("Marking");
						}
						else
						{
#if UNITY_EDITOR
							Debug.Log("DogRushingAndMarking->AIUpdate: if (!MainGameManager.instance.isGameEnd)");
 #endif
						}

						m_markingEffect.SetActive(true);
						functionState = State.Marking;
						timer.Start();
					}

					//回転
					transform.rotation = Quaternion.Slerp(rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
					break;
				}
			//マーキング実行
			case State.Marking:
				{
					//指定時間経過
					if (timer.elapasedTime >= m_markingSeconds)
					{
						functionState = State.MarkingEnd;
						m_markingEffect.SetActive(false);
						m_sePlayer.Stop(m_markingSEIndex);

						if (!MainGameManager.instance.isGameEnd 
							&& m_bgmChangeAgentID == dogAIAgent.aiAgentInstanceID)
						{
							m_bgmChangeAgentID = -1;
							AudioManager.instance.FadeinBgm("Marking");
						}
						else
						{
#if UNITY_EDITOR
							Debug.Log("DogRushingAndMarking->AIUpdate: if (!MainGameManager.instance.isGameEnd)" +
								"\n && m_bgmChangeAgentID == dogAIAgent.aiAgentInstanceID");
#endif
						}
						//Animation Set
						m_animationController.editAnimation.TriggerMarkingEnd();
					}

					break;
				}
			//マーキング終了→関数終了
			case State.FunctionEnd:
				{
					m_markPoint.LinkMarkingEnd();

					//Animation Set
					m_animationController.editAnimation.isWakeUp = false;
					m_animationController.editAnimation.TriggerWaitRunStart();
					//終了
					EndAIFunction(updateIdentifier);
					break;
				}
			default:
				break;
		}
	}
}
