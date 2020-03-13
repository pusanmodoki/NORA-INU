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
	/// マークポイントまで移動するGoingMarkPoint
	/// </summary>
	public class GoingMarkPoint : BaseAIFunction
	{
		public Transform markTarget { get { return m_target; } }

		/// <summary>This visibility</summary>
		[SerializeField, Header("Reference"), Tooltip("This visibility")]
		AIVisibility m_visibility = null;
		/// <summary>マーキング実行可能範囲に入った場合実行する関数</summary>
		[SerializeField, Tooltip("マーキング実行可能範囲に入った場合実行する関数")]
		BaseAIFunction m_function = null;
		/// <summary>Overlap layer mask</summary>
		[SerializeField, Header("Overlap"), Tooltip("Overlap layer mask")]
		LayerMaskEx m_overlapLayerMask = 0;
		/// <summary>Overlap radius</summary>
		[SerializeField, Tooltip("Overlap radius")]
		float m_overlapRadius = 2.0f;
		[SerializeField, Header("Other"), Tooltip("Timeout seconds")]
		float m_timeoutSeconds = 5.0f;

		//debug only
#if UNITY_EDITOR
		/// <summary>Visibility is draw gizmos? (debug only)</summary>
		[SerializeField, Header("Debug Only"), Tooltip("Visibility is draw gizmos? (debug only)")]
		bool m_dIsDrawGizmos = true;

		/// <summary>Hit color (debug only)</summary>
		static readonly Color m_dGizmoHitColor = new Color(0.7f, 0.7f, 0.7f);
		/// <summary>Not hit color (debug only)</summary>
		static readonly Color m_dGizmoNotHitColor = new Color(0.2f, 0.2f, 0.2f);
#endif

		/// <summary>Target transform</summary>
		Transform m_target = null;


		/// <summary>
		/// [AIBegin]
		/// 関数初回実行時に呼ばれるコールバック関数
		/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
		/// 引数2: 並列関数として実行？
		/// </summary>
		public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
		{
			//視界判定をとる
			m_visibility.IsHitVisibility();
			m_target = m_visibility.lookTarget;

			//見つかっていれば目標地点を設定
			if (m_target != null)
			{
				SetUpdatePosition(true);
				navMeshAgent.destination = m_target.position;
			}
		}
		/// <summary>
		/// [AIEnd]
		/// 関数が実行登録を解除される際に呼ばれるコールバック関数
		/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
		/// 引数2: 並列関数として実行？
		/// </summary>
		public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
		{
			//見つかっていれば移動停止
			if (m_target != null)
			{
				SetUpdatePosition(false);
			}
		}

		/// <summary>
		/// [AIUpdate]
		/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
		/// 引数1: 更新識別子
		/// </summary>
		public override void AIUpdate(UpdateIdentifier updateIdentifier)
		{
			if (m_target == null)
			{
#if UNITY_EDITOR
				Debug.LogError("Error!! GoingMarkingPoint->AIUpdate, target == null");
#endif
				EndAIFunction(updateIdentifier);
			}

			if (timer.elapasedTime > m_timeoutSeconds)
				EndAIFunction(updateIdentifier);

			//マーキング実行範囲に入ったか判定する
			var collisions = Physics.OverlapSphere(transform.position, m_overlapRadius, m_overlapLayerMask);

			if (collisions.Length == 0)
				return;

			//該当しているか検索ループ
			foreach (var e in collisions)
			{
				Transform useTransform = e.gameObject.transform;
				ProvideMainObject provideMainObject = useTransform.GetComponent<ProvideMainObject>();

				if ((provideMainObject != null
						&& m_target.GetInstanceID() == provideMainObject.mainObject.GetInstanceID())
					|| provideMainObject == null
						&& m_target.GetInstanceID() == useTransform.GetInstanceID())
				{
					aiAgent.ForceSpecifyFunction(m_function);
				}
			}
		}


		//debug only
#if UNITY_EDITOR
		///<summary>[OnDrawGizmos]</summary>
		void OnDrawGizmos()
		{
			if (!m_dIsDrawGizmos)
				return;

			if (Physics.CheckSphere(transform.position, m_overlapRadius, m_overlapLayerMask))
				Gizmos.color = m_dGizmoHitColor;
			else
				Gizmos.color = m_dGizmoNotHitColor;

			Gizmos.DrawWireSphere(transform.position, m_overlapRadius);
		}
#endif
	}
}