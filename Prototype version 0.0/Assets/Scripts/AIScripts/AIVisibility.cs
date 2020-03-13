//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy AI Components 
/// </summary>
namespace AIComponent
{
    /// <summary>
    /// 視界判定を行うAIVisibility class
    /// </summary>
    public class AIVisibility : MonoBehaviour
    {
        /// <summary>AI agent</summary>
        public AIAgent aiAgent { get { return m_agent; } }
		/// <summary>Look target transform</summary>
		public Transform lookTarget { get { return m_lookObject != null ? m_lookObject.transform : null; } }
		/// <summary>RaycastHit</summary>
		public RaycastHit raycastHit { get { return m_raycastHit; } }
		/// <summary>Distance visibility</summary>
		public float distanceVisibility { get { return m_distance; } }
        /// <summary>Angle visibility</summary>
        public float angleVisibility { get { return m_angle; } }
        /// <summary>Visibility height Limit</summary>
        public float heightLimitVisibility { get { return m_heightLimit; } }
        /// <summary>Lose sight distance</summary>
        public float loseSightDistance { get { return m_loseSightDistance; } }
		/// <summary>Lose sight distance</summary>
		public float overlapRadius { get { return m_overlapRadius; } }

		/// <summary>AIAgent</summary>
		[SerializeField, Header("AI Agent"), Tooltip("AI agent")]
        AIAgent m_agent = null;
        /// <summary>見失った際のコールバック</summary>
        [SerializeField, Tooltip("見失った際のコールバック")]
        BaseAIFunction m_loseSightCallback = null;
		/// <summary>Found!!->AIAgent.AllocateFunction running</summary>
		[SerializeField, Tooltip("Found!!->AIAgent.AllocateFunction running")]
        bool m_isWhenFoundAllocate = false;

        /// <summary>SE player</summary>
        [SerializeField, Header("Audio"), Tooltip("SE player"), Space]
        SEPlayer m_sePlayer = null;
		/// <summary>Hit!!->play se index</summary>
		[SerializeField, Tooltip("Hit!!->play se index")]
        int m_indexOfSE = 0;
		/// <summary>If (hit->play) -> stop all se</summary>
		[SerializeField, Tooltip("If (hit->play) -> stop all se")]
        bool m_isStopWhenPlayingSE = false;
        /// <summary>SE looping?</summary>
        [SerializeField, Tooltip("SE looping?")]
        bool m_isLoopSE = false;

        /// <summary>Visibility Distance (forward)</summary>
        [SerializeField, Header("Visibility"), Tooltip("Visibility distance (forward)")]
        float m_distance = 10;
        /// <summary>Visibility angle</summary>
        [SerializeField, Range(0.0f, 180.0f), Tooltip("Visibility angle")]
        float m_angle = 90;
		/// <summary>Visibility height limit (Y axis [this + -max] ~ [this + max])</summary>
		[SerializeField, Tooltip("Visibility height limit (Y axis [this + -max] ~ [this + max])")]
        float m_heightLimit = 5;
        /// <summary>lose sight distance</summary>
        [SerializeField, Tooltip("Lose sight distance")]
        float m_loseSightDistance = 5.0f;
        /// <summary>lose sight distance</summary>
        [SerializeField, Tooltip("Timeout in lose sight distance->lose sight")]
        float m_timeoutInLoseSight = 5.0f;

		/// <summary>Raycast LayerMask</summary>
		[SerializeField, Header("Raycast And Overlap"), Tooltip("Raycast and overlap layer mask")]
        LayerMaskEx m_layerMask = int.MaxValue;
		/// <summary>Raycast center position</summary>
		[SerializeField, Tooltip("Raycast center position")]
        Vector3 m_raycastCenter = Vector3.zero;
		/// <summary>Overlap center position</summary>
		[SerializeField, Tooltip("Overlap center position")]
		Vector3 m_overlapCenter = Vector3.zero;
		/// <summary>Overlap sphere radius</summary>
		[SerializeField, Tooltip("Overlap sphere radius")]
		float m_overlapRadius = 10.0f;

		/// <summary>追跡中オブジェクト</summary>
		GameObject m_lookObject = null;
		/// <summary>追跡中オブジェクト->Component</summary>
		ProvideMainObject m_provideMainObject = null;
		/// <summary>RaycastHit</summary>
		RaycastHit m_raycastHit = new RaycastHit();
		/// <summary>Lose Sight Timer</summary>
		Timer m_loseSightTimer = new Timer();
        /// <summary>Angle->Radian->Cos</summary>
        float m_angleToCosine = 0.0f;
        /// <summary>現在フレーム判定済み？</summary>
        bool m_isNowFrameCompleted = false;
        /// <summary>result</summary>
        bool m_isResult = false;
        /// <summary>Old found info</summary>
        bool m_isOldFound = false;

        //debug only
#if UNITY_EDITOR
        /// <summary>Draw gizmo mesh (debug only)</summary>
        public Mesh dGizmoMesh { get; set; } = null;
        /// <summary>Draw gizmo mesh iD (debug only)</summary>
        public int dMeshID { get; set; } = -1;
        /// <summary>Visibility is hit? (debug only)</summary>
        public bool dIsHit { get { return m_dIsHit; } }
        /// <summary>Visibility is draw gizmos? (debug only)</summary>
        public bool dIsDrawGizmos { get { return m_dIsDrawGizmos; } set { m_dIsDrawGizmos = value; } }

        /// <summary>Visibility is draw gizmos? (debug only)</summary>
        [SerializeField, Header("Debug Only"), Tooltip("Visibility is draw gizmos? (debug only)")]
        bool m_dIsDrawGizmos = true;
        /// <summary>Visibility is hit? (debug only)</summary>
        [SerializeField, Tooltip("Visibility is hit? (debug only)")]
        bool m_dIsHit = false;
        /// <summary>Visibility is draw gizmos? (debug only)</summary>
        [SerializeField, Tooltip("Visibility drawing lose sight timer (debug only)")]
        float m_dLoseSightTimer = 0.0f;
#endif

		/// <summary>
		/// [IsHitVisibility]
		/// ターゲットが視界にいるか判定する
		/// return: 視界にいると判定した場合true
		/// 引数1: 現在フレーム判定済みでも強制的に行うか
		/// </summary>
		public bool IsHitVisibility(bool isForced = false)
		{
			//既に現在フレーム判定済みならそのまま返す
			if (m_isNowFrameCompleted & !isForced)
				return m_isResult;

			//フラグ初期化
			m_isResult = true;
			m_isNowFrameCompleted = true;
			//debug only            
#if UNITY_EDITOR
			//ヒットフラグ初期化
			m_dIsHit = false;
			m_dLoseSightTimer = m_loseSightTimer.elapasedTime;
#endif

			//呼び出しコスト削減
			Vector3 position = transform.position;
			Vector3 raycastCenter, overlapCenter;

			//Center計算
			CalculateCenters(position, out raycastCenter, out overlapCenter);

			//overlap
			var collisions = Physics.OverlapSphere(overlapCenter, m_overlapRadius, m_layerMask);
			//not hit -> return false
			if (collisions.Length == 0)
			{
				m_lookObject = null;
				return m_isResult = false;
			}

			//ソートとかすんのかな
			//とりあえず追跡してなかったら一番上のものを使うぜ	
			GameObject useCollisionObject = collisions[0].gameObject;
			Vector3 targetPosition;

			//追跡してたら追跡オブジェクトにしちゃうんだな
			if (m_lookObject != null)
			{
				useCollisionObject = m_lookObject;
				targetPosition = m_provideMainObject != null ?
					m_provideMainObject.mainObject.transform.position : useCollisionObject.transform.position;
			}
			//そうじゃなかったら基準点を探す
			else
			{
				ProvideMainObject provideMainObject = useCollisionObject.GetComponent<ProvideMainObject>();
				targetPosition = provideMainObject != null ?
					provideMainObject.mainObject.transform.position : useCollisionObject.transform.position;

				//ここでしか登録できないしとりあえずここで登録しておく
				m_provideMainObject = provideMainObject;
			}

			//自分へのターゲットから方向ベクトル
			Vector3 raycastDirection = (targetPosition - raycastCenter).normalized;

			//高さが設定以上離れている or 距離が設定以上離れている->result = false
			if (Mathf.Abs(position.y - targetPosition.y) > m_heightLimit
				|| (targetPosition - position).sqrMagnitude > distanceVisibility * distanceVisibility)
				m_isResult = false;

			//同位置？
			if (m_isResult && raycastDirection.sqrMagnitude < Mathf.Epsilon)
			{
				//とりあえずHit用にRaycast
				Physics.Raycast(raycastCenter, raycastDirection, out m_raycastHit, m_distance, m_layerMask);

				//debug only
#if UNITY_EDITOR
				//ヒットフラグ = true
				m_dIsHit = true;
#endif
				//見失ってるTimerStart, どちらかというと見つかったらリセットしているて方が近い
				m_loseSightTimer.Start();
				//Look object 登録
				if (m_lookObject == null)
					m_lookObject = useCollisionObject;
				//フラグ = true
				return m_isResult = true;
			}

			//Dotで視界角度内にいるか判定, 居ない->result = false
			if (m_isResult && Vector3.Dot(transform.forward, raycastDirection) < m_angleToCosine)
				m_isResult = false;

			//Raycast, ヒットしない->result = false
			if (m_isResult && !Physics.Raycast(raycastCenter, raycastDirection, out m_raycastHit, m_distance, m_layerMask))
				m_isResult = false;

			//Raycastにヒットした物体がTargetの場合ヒットしたとする->result = false
			//この時点で見つけたかが確定する
			if (m_isResult
				&& m_raycastHit.transform.gameObject.layer != useCollisionObject.gameObject.layer)
				m_isResult = false;

			//見つけてたら見失ってるTimerStart
			//どちらかというと見つけたらリセットしているて方が近い
			if (m_isResult)
				m_loseSightTimer.Start();

			//Hit or 前フレーム見つけていたら距離 & 時間判定をとる -> 許容範囲内
			if (m_isResult || (m_isOldFound
				&& (targetPosition - position).sqrMagnitude < m_loseSightDistance * m_loseSightDistance
				&& m_loseSightTimer.elapasedTime <= m_timeoutInLoseSight))
			{
				//debug only
#if UNITY_EDITOR
				//ヒットフラグ = true
				m_dIsHit = true;
#endif
				//Look object 登録
				if (m_lookObject == null)
					m_lookObject = useCollisionObject;
				//フラグ = true
				return m_isResult = true;
			}
			//見つからんかった
			else
			{
				m_lookObject = null;
				return m_isResult = false;
			}
		}

		/// <summary>
		/// [CalculateCenters]
		/// Raycast, Overlapで使用するCenterを計算する
		/// 引数1: position + centerで使用するthis position	
		/// 引数2(out): raycast center
		/// 引数3(out): overlap center
		/// </summary>
		public void CalculateCenters(Vector3 position, out Vector3 raycast, out Vector3 overlap)
		{
			Transform thisTransform = transform;
			Vector3 right = thisTransform.right,
				up = thisTransform.up, forward = thisTransform.forward;

			raycast = position + m_raycastCenter.x * right + m_raycastCenter.y * up + m_raycastCenter.z * forward;
			overlap = position + m_overlapCenter.x * right + m_overlapCenter.y * up + m_overlapCenter.z * forward;
		}

		/// <summary>[Start]</summary>
		void Start()
		{
			//視界角度を変換
			m_angleToCosine = Mathf.Cos(m_angle * 0.5f * Mathf.Deg2Rad);
		}
		/// <summary>[OnValidate]</summary>
		void OnValidate()
		{
			//視界角度を変換
			m_angleToCosine = Mathf.Cos(m_angle * 0.5f * Mathf.Deg2Rad);

			//debug only
#if UNITY_EDITOR
			//meshを変更するのでnullに
			dGizmoMesh = null;
#endif
		}
		/// <summary>[LateUpdate]</summary>
		void Update()
		{
			//見つけたら再Allocate?
			if (m_isWhenFoundAllocate)
			{
				bool found = IsHitVisibility();

				if (((found ^ m_isOldFound) & found) != false)
					m_agent.AllocateFunction();
			}
			//見失ったらCallback?
			if (m_loseSightCallback != null)
			{
				bool found = IsHitVisibility();

				if (((found ^ m_isOldFound) & m_isOldFound) != false)
					m_agent.ForceSpecifyFunction(m_loseSightCallback);
			}
		}
		/// <summary>[LateUpdate]</summary>
		void LateUpdate()
		{
			bool found = IsHitVisibility();

			//見つけとったら音を流す
			if (((found ^ m_isOldFound) & found) != false && m_sePlayer != null)
			{
				if (m_isStopWhenPlayingSE)
					m_sePlayer.StopAll();

				m_sePlayer.PlaySE(m_indexOfSE, m_isLoopSE);
			}

			//フラグ初期化
			m_isOldFound = m_isResult;
			m_isNowFrameCompleted = false;
		}
	}
}