//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RaycastをカスタムしたRaycastFlags
/// </summary>
[DefaultExecutionOrder(-100)]
public class RaycastFlags : MonoBehaviour
{
    /// <summary>
    /// Raycastの方向を示すRaycastDirection
    /// </summary>
    enum RaycastDirection
    {
        /// <summary>transform.forward</summary>
        Forward = 0x12,
        /// <summary>-transform.forward</summary>
        Back = 0x10,
        /// <summary>transform.up</summary>
        Up = 0x22,
        /// <summary>-transform.up</summary>
        Down = 0x20,
        /// <summary>transform.right</summary>
        Right = 0x42,
        /// <summary>-transform.right</summary>
        Left = 0x40,
        /// <summary>target transform direction</summary>
        Target = 0x80
    }

    /// <summary>
    /// Raycastの結果を格納するRaycastResult
    /// </summary>
    public class RaycastResult
    {
        /// <summary>コンストラクタ</summary>
        public RaycastResult(Transform transform, Vector3 position, Vector3 normal,
            Vector3 direction, LayerMaskEx layerMask, float distance)
        {
            this.transform = transform;
            this.position = position;
            this.normal = normal;
            this.direction = direction;
            this.layerMask = layerMask;
            this.distance = distance;
        }

        /// <summary>ヒットしたTransform</summary>
        public Transform transform;
        /// <summary>ヒットした座標</summary>
        public Vector3 position;
        /// <summary>ヒットした面の法線</summary>
        public Vector3 normal;
        /// <summary>Rayの方向</summary>
        public Vector3 direction;
        /// <summary>ヒットしたオブジェクトのレイヤーマスク</summary>
        public LayerMaskEx layerMask;
        /// <summary>ヒットした座標までの距離</summary>
        public float distance;
    }

    /// <summary>
    /// RaycastDirectionをビット変数として格納したRaycastDirectionBits
    /// </summary>
    struct RaycastDirectionBits
    {
        /// <summary>transform.forward or -transform.forward</summary>
        public static readonly int cForwardBit = 0x10;
        /// <summary>transform.forward</summary>
        public static readonly int cForward = 0x12;
        /// <summary>-transform.forward</summary>
        public static readonly int cBack = 0x10;

        /// <summary>transform.up or -transform.up</summary>
        public static readonly int cUpBit = 0x20;
        /// <summary>transform.up</summary>
        public static readonly int cUp = 0x22;
        /// <summary>-transform.up</summary>
        public static readonly int cDown = 0x20;

        /// <summary>transform.right or -transform.right</summary>
        public static readonly int cRightBit = 0x40;
        /// <summary>transform.right</summary>
        public static readonly int cRight = 0x42;
        /// <summary>-transform.right</summary>
        public static readonly int cLeft = 0x40;
        
        /// <summary>target transform direction</summary>
        public static readonly int cTarget = 0x80;
    }

    /// <summary>Raycast enabled</summary>
    public bool isEnabledRaycast { get { return m_isEnabledRaycast; } set { m_isEnabledRaycast = value; if (value) Update(); } }
    /// <summary>Raycast result flag->Always true on hit (other object)</summary>
    public bool isOtherObjectHit { get; protected set; } = false;
    /// <summary>Raycast result flag->Always true on hit</summary>
    public bool isStay { get; protected set; } = false;
    /// <summary>Raycast result flag->True in the hit frame</summary>
    public bool isEnter { get; protected set; } = false;
    /// <summary>Raycast result flag->True in the not hit frame</summary>
    public bool isExit { get; protected set; } = false;
    /// <summary>Raycast result infos (warning! not hit frame = null)</summary>
    public RaycastResult rayCastResult { get; private set; } = null;
    /// <summary>traget in target mode of raycast</summary>
    public Transform target { get { return m_target; } set { m_target = value; } }
    /// <summary>Raycast Center Position</summary>
    public Vector3 centerPosition { get; private set; } = Vector3.zero;
	public Vector3 lockDirection { get; private set; } = Vector3.zero;
	/// <summary>Distance of Raycast</summary>
	public float distance { get { return m_distance; } }
    /// <summary>Direction Lock of Raycast</summary>
    public bool isDirectionLock { get { return m_isDirectionLock; } set { m_isDirectionLock = value; } }

    /// <summary>Center of Raycast</summary>
    [Tooltip("Center of Raycast"), SerializeField]
    Vector3 m_center = Vector3.zero;
    /// <summary>Direction of Raycast</summary>
    [Header("Raycast Info"), Tooltip("Direction of Raycast"), SerializeField]
    RaycastDirection m_direction = RaycastDirection.Down;
    /// <summary>traget in target mode of raycast</summary>
    [Tooltip("traget in target mode of raycast"), SerializeField]
    Transform m_target = null;
    /// <summary>LayerMask of Raycast</summary>
    [Tooltip("LayerMask of Raycast"), SerializeField]
    LayerMaskEx m_raycastLayerMask = int.MaxValue;
    /// <summary>LayerMask of Target Mode Raycast</summary>
    [Tooltip("LayerMask of Target Mode Raycast"), SerializeField]
    LayerMaskEx m_raycastTargetLayerMask = int.MaxValue;
    /// <summary>Distance of Raycast</summary>
    [Tooltip("Distance of Raycast"), SerializeField]
    float m_distance = 1.0f;
    /// <summary>Enabled of Raycast</summary>
    [Tooltip("Enabled of Raycast"), SerializeField]
    bool m_isEnabledRaycast = true;
    /// <summary>Direction Lock of Raycast</summary>
    [Tooltip("Direction Lock of Raycast"), SerializeField]
    bool m_isDirectionLock = false;

    //debug only
#if UNITY_EDITOR
    /// <summary>gizmo color -> hit frame</summary>
    static readonly Color m_cdHitColor = new Color(1.0f, 0.15f, 0.1f, 0.8f);
    /// <summary>gizmo color -> not hit frame</summary>
    static readonly Color m_cdNotHitColor = new Color(0.95f, 0.95f, 0.95f, 0.8f);

    /// <summary>Draw Gizmos? (debug only)</summary>
    [SerializeField, Tooltip("Draw Gizmos? (debug only)"), Header("Debug Only")]
    bool m_dIsDrawGizmos = false;
    /// <summary>Drawing isEnabledRaycast property (debug only)</summary>
    [SerializeField, Tooltip("Drawing isEnabledRaycast property (debug only)")]
    bool m_dIsEnabled = false;
    /// <summary>Drawing isStay property (debug only)</summary>
    [SerializeField, Tooltip("Drawing isStay property (debug only)")]
    bool m_dIsStayNow = false;
    /// <summary>Drawing isEnter property (debug only)</summary>
    [SerializeField, Tooltip("Drawing isEnter property (debug only)")]
    bool m_dIsEnterNow = false;
    /// <summary>Drawing isExit property (debug only)</summary>
    [SerializeField, Tooltip("Drawing isExit property (debug only)")]
    bool m_dIsExitNow = false;
#endif

	bool m_isOldDirectionLock = false;

	/// <summary>
	/// [RaycastDirection]
	/// return: m_directionに沿ったRaycastの方向
	/// </summary>
	public Vector3 GetRaycastDirection(Vector3 center = new Vector3())
    {
        //intに変換
        int toBit = (int)m_direction;

        //各ビットでどの方向変数を使うか求めた後、-1 or 1で掛け算して返却
        if ((toBit & RaycastDirectionBits.cForwardBit) != 0)
            return transform.forward * ((toBit & 0x2) - 1);
        else if ((toBit & RaycastDirectionBits.cUpBit) != 0)
            return transform.up * ((toBit & 0x2) - 1);
        else if ((toBit & RaycastDirectionBits.cRightBit) != 0)
            return transform.right * ((toBit & 0x2) - 1);
        else if ((toBit & RaycastDirectionBits.cTarget) != 0)
            return (m_target.position - center).normalized;
        else
            return Vector3.zero;
    }

    /// <summary>[Update]</summary>
    void Update()
    {	
		//フラグ計算に使用
        bool isOldStay = isStay;
        isOtherObjectHit = false;

        if (!isEnabledRaycast)
        {
            //フラグ計算に使用
            isStay = false;

            //isEnter Frame?
            if ((isStay ^ isOldStay) & isStay)
            {
                isEnter = true;
                isExit = false;
            }
            //isExit Frame?
            else if ((isStay ^ isOldStay) & isOldStay)
            {
                isEnter = false;
                isExit = true;
            }

            if (!isStay & !isEnter & !isExit)
                rayCastResult = null;

            //debug only
#if UNITY_EDITOR
            //表示用変数更新
            m_dIsStayNow = isStay;
            m_dIsEnterNow = isEnter;
            m_dIsExitNow = isExit;
            m_dIsEnabled = isEnabledRaycast;
#endif

            return;
        }
        
        //呼び出しコスト削減
        Transform myTransform = transform;
        Quaternion rotation = myTransform.rotation;
        Vector3 lossyScale = myTransform.lossyScale;
        Vector3 position = myTransform.position;
        
        //RaycastHit
        RaycastHit raycastHit = new RaycastHit();

		//center
		centerPosition = position + myTransform.right * m_center.x + myTransform.up * m_center.y + myTransform.forward * m_center.z;
		
		//direction
		Vector3 direction = GetRaycastDirection(centerPosition);
        if (m_isDirectionLock && rayCastResult != null)
            direction = rayCastResult.direction;

        //Raycast
        isStay = Physics.Raycast(centerPosition, direction, out raycastHit, m_distance, m_raycastLayerMask);

        //Hit!
        if (isStay && m_raycastTargetLayerMask.EqualBitsForGameObject(raycastHit.transform.gameObject))
        {
            //result代入
            rayCastResult = new RaycastResult(raycastHit.transform, raycastHit.point, raycastHit.normal, direction,
                1 << raycastHit.transform.gameObject.layer, raycastHit.distance);
        }
        else if (isStay)
        {
            isStay = false;
            isOtherObjectHit = true;
            //result代入
            rayCastResult = new RaycastResult(raycastHit.transform, raycastHit.point, raycastHit.normal, direction,
                1 << raycastHit.transform.gameObject.layer, raycastHit.distance);
        }
        else
            rayCastResult = null;

        //isEnter Frame?
        if ((isStay ^ isOldStay) & isStay)
        {
            isEnter = true;
            isExit = false;
        }
        //isExit Frame?
        else if ((isStay ^ isOldStay) & isOldStay)
        {
            isEnter = false;
            isExit = true;
        }
        //isStay Frame
        else
        {
            isEnter = false;
            isExit = false;
        }

		
		if (((m_isOldDirectionLock ^ isDirectionLock) & isDirectionLock)
			| !isDirectionLock)
			lockDirection = direction;

		//debug only
#if UNITY_EDITOR
		//表示用変数更新
		m_dIsStayNow = isStay;
        m_dIsEnterNow = isEnter;
        m_dIsExitNow = isExit;
        m_dIsEnabled = isEnabledRaycast;
#endif
    }

    //debug only
#if UNITY_EDITOR
    /// <summary>[OnDrawGizmos]</summary>
    void OnDrawGizmos()
    {
        //!Flgな場合終了
        if (!m_dIsDrawGizmos) return;

        //描画に反映されないため実行中以外はこちらで更新を行う
        if (!UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused)
            Update();

        //呼び出しコスト削減
        Transform myTransform = transform;
        //取得
        Vector3 direction = GetRaycastDirection(centerPosition);
        //Hit
        if (isStay)
        {
            //Rayの距離を求める
            float distance = (rayCastResult != null) ? rayCastResult.distance : m_distance;
            //Color
            Gizmos.color = m_cdHitColor;
            //Draw Ray
            Gizmos.DrawRay(centerPosition, direction * distance);
        }
        //Not Hit
        else
        {
            //Color
            Gizmos.color = m_cdNotHitColor;
            //Draw Ray
            Gizmos.DrawRay(centerPosition, direction * m_distance);
        }
	}
#endif
}
