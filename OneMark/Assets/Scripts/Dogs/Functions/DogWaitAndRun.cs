using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class DogWaitAndRun : BaseDogAIFunction
{
	/// <summary>移動時間</summary>
	[SerializeField, Tooltip("移動時間")]
	float m_moveSeconds = 0.1f;
	[SerializeField]
	float m_updateDestinationInterval = 0.05f;
	[SerializeField]
	float m_rotationOffset = 90.0f;
	[SerializeField]
	float m_rotationSpeed = 10.0f;

	Timer m_moveTimer = new Timer();
	Timer m_updateDestinationTimer = new Timer();
	Quaternion m_targetRotation = Quaternion.identity;
	Vector3 m_markPointPosition = Vector3.zero;
	Vector3 m_forwardMultiPointDistance = Vector3.zero;
	float m_pointDistance = 0.0f;
	bool m_isWakeUp = false;

	/// <summary>
	/// [AIBegin]
	/// 関数初回実行時に呼ばれるコールバック関数
	/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// 引数2: 並列関数として実行？
	/// </summary>
	public override void AIBegin(BaseAIFunction beforeFunction)
	{
		if (dogAIAgent.linkMarkPoint == null) return;

		m_moveTimer.Start();
		m_updateDestinationTimer.Start();
		if (beforeFunction == this) return;

		m_pointDistance = dogAIAgent.linkMarkPoint.localMarkingTarget.magnitude;
		m_forwardMultiPointDistance = Vector3.forward * m_pointDistance;
		m_markPointPosition = dogAIAgent.linkMarkPoint.transform.position;

		m_targetRotation = Quaternion.FromToRotation(Vector3.forward,
			dogAIAgent.linkMarkPoint.localMarkingTarget.normalized) * Quaternion.AngleAxis(m_rotationOffset, Vector3.up);

		navMeshAgent.SetDestination(m_markPointPosition + m_targetRotation * m_forwardMultiPointDistance);

		dogAIAgent.animationController.editAnimation.isWakeUp = false;
		m_isWakeUp = false;
	}
	/// <summary>
	/// [AIEnd]
	/// 関数が実行登録を解除される際に呼ばれるコールバック関数
	/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
	/// 引数2: 並列関数として実行？
	/// </summary>
	public override void AIEnd(BaseAIFunction nextFunction)
	{
		if (dogAIAgent.linkMarkPoint == null) return;
		navMeshAgent.isStopped = false;
	}
	/// <summary>
	/// [AIUpdate]
	/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
	/// 引数1: 更新識別子
	/// </summary>
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		if (dogAIAgent.linkMarkPoint == null)
		{
			EndAIFunction(updateIdentifier);
			return;
		}
		if (m_moveTimer.elapasedTime > m_moveSeconds)
		{
			EndAIFunction(updateIdentifier);
			return;
		}

		if (dogAIAgent.linkMarkPoint == dogAIAgent.linkPlayerInfo.manualCollisionAdministrator.hitVisibilityMarkPoint
			&& dogAIAgent.linkMarkPoint != null)
		{
			if (!m_isWakeUp)
			{
				dogAIAgent.animationController.editAnimation.isWakeUp = true;
				m_isWakeUp = true;
				navMeshAgent.isStopped = true;
				m_updateDestinationTimer.Stop();
			}
		}
		else
		{
			if (m_isWakeUp)
			{
				dogAIAgent.animationController.editAnimation.isWakeUp = false;
				m_isWakeUp = false;
				navMeshAgent.isStopped = false;
			}

			m_targetRotation *= Quaternion.AngleAxis(m_rotationSpeed * Time.deltaTime, Vector3.up);
			if (m_updateDestinationTimer.elapasedTime > m_updateDestinationInterval)
			{
				navMeshAgent.SetDestination(m_markPointPosition + m_targetRotation * m_forwardMultiPointDistance);
				m_updateDestinationTimer.Start();
			}
		}
	}
}
