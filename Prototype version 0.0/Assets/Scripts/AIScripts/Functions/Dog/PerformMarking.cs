//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
	/// <summary>
	/// マークポイントを実行するPerformMarking
	/// </summary>
	public class PerformMarking : BaseAIFunction
	{
		enum State
		{
			Rotation,
			Marking,
		}

		class MarkPointInfo
		{
			public MarkPointInfo(Transform transform,
				MarkingMessage message, Transform thisTransform, Vector3 goalRotation)
			{
				this.transform = transform;
				this.message = message;
				this.goalRotation = Quaternion.LookRotation((transform.position - thisTransform.position).normalized, thisTransform.up) * Quaternion.Euler(goalRotation);
				this.position = transform.position;
			}

			public Transform transform;
			public MarkingMessage message;
			public Quaternion goalRotation;
			public Vector3 position;
		}

		public bool isNowMarking { get { return timer.isStart; } }

		[SerializeField]
		GameObject m_rotationApplyObject = null;
		[SerializeField]
		GoingMarkPoint m_thisGoingFunction = null;
		[SerializeField]
		KamikazeCommand m_kamikazeCommand = null;
		
		[SerializeField]
		Vector3 m_markingRotation = Vector3.zero;
		[SerializeField]
		float m_attack = 1.0f;
		[SerializeField]
		float m_rotationSpeed = 0.9f;
		[SerializeField]
		float m_rotationTime = 1.0f;

		MarkPointInfo m_markingObjectInfo = null;
		Transform m_markTarget = null;
		State m_state;

		/// <summary>
		/// [AIBegin]
		/// 関数初回実行時に呼ばれるコールバック関数
		/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
		/// 引数2: 並列関数として実行？
		/// </summary>
		public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
		{
			m_markTarget = m_thisGoingFunction.markTarget;
			if (m_markTarget == null) return;

			var message = m_markTarget.GetComponent<MarkingMessage>();
			if (message == null) return;

			m_markingObjectInfo = new MarkPointInfo(m_markTarget,
				message, transform, m_markingRotation);
		}
		/// <summary>
		/// [AIEnd]
		/// 関数が実行登録を解除される際に呼ばれるコールバック関数
		/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
		/// 引数2: 並列関数として実行？
		/// </summary>
		public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
		{
			m_markTarget = null;
			m_markingObjectInfo = null;
		}

		/// <summary>
		/// [AIUpdate]
		/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
		/// 引数1: 更新識別子
		/// </summary>
		public override void AIUpdate(UpdateIdentifier updateIdentifier)
		{
			if (m_markTarget == null || m_markingObjectInfo == null)
			{
				EndAIFunction(updateIdentifier);
				return;
			}

			if (timer.elapasedTime >= m_rotationTime)
			{
				m_markingObjectInfo.message.SendMessage(new MarkerInfo(m_attack));
				m_kamikazeCommand.EndKamikaze();
				EndAIFunction(updateIdentifier);
				return;
			}

			m_rotationApplyObject.transform.rotation =
				Quaternion.Slerp(transform.rotation, m_markingObjectInfo.goalRotation, m_rotationSpeed * Time.deltaTime);			
		}
	}
}
