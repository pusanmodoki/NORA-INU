//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BoxCastを行い結果を保存するBoxCastFlags class
/// </summary>
[DefaultExecutionOrder(-100)]
public class BoxCastFlags : MonoBehaviour
{
    /// <summary>
    /// BoxCastの方向を示すBoxCastDirection
    /// </summary>
    enum BoxCastDirection
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
    /// BoxCastの結果を格納するBoxCastResult
    /// </summary>
    public class BoxCastResult
    {
        /// <summary>コンストラクタ</summary>
        public BoxCastResult(Transform transform, Vector3 position, Vector3 normal,
            LayerMaskEx layerMask, float distance, bool isResultOfRaycast)
        {
			this.transform = transform;
			this.position = position;
            this.normal = normal;
            this.layerMask = layerMask;
            this.distance = distance;
            this.isResultOfRaycast = isResultOfRaycast;
        }

		/// <summary>ヒットしたオブジェクト</summary>
		public Transform transform;
		/// <summary>ヒットした座標</summary>
		public Vector3 position;
        /// <summary>ヒットした面の法線</summary>
        public Vector3 normal;
        /// <summary>ヒットしたオブジェクトのレイヤーマスク</summary>
        public LayerMaskEx layerMask;
        /// <summary>ヒットした座標までの距離</summary>
        public float distance;
        /// <summary>BoxCastでの結果->true, CheckBoxでの結果->false</summary>
        public bool isResultOfRaycast;
    }

    /// <summary>
    /// BoxCastDirectionをビット変数として格納したBoxCastDirectionBits
    /// </summary>
    struct BoxCastDirectionBits
    {
        /// <summary>transform.forward or -transform.forward</summary>
        public static readonly int cForwardBit = 0x10;
        /// <summary>transform.forward</summary>
        public static readonly int cForward = 0x12;
        /// <summary>-transform.forward</summary>
        public static readonly int Back = 0x10;

        /// <summary>transform.up or -transform.up</summary>
        public static readonly int cUpBit = 0x20;
        /// <summary>transform.up</summary>
        public static readonly int Up = 0x22;
        /// <summary>-transform.up</summary>
        public static readonly int Down = 0x20;

        /// <summary>transform.right or -transform.right</summary>
        public static readonly int cRightBit = 0x40;
        /// <summary>transform.right</summary>
        public static readonly int Right = 0x42;
        /// <summary>-transform.right</summary>
        public static readonly int Left = 0x40;

		/// <summary>target transform direction</summary>
		public static readonly int cTarget = 0x80;
	}
	/// <summary>BoxCast result flag->Always true on hit</summary>
	public bool isStay { get; protected set; } = false;
	/// <summary>BoxCast result flag->True in the hit frame</summary>
	public bool isEnter { get; protected set; } = false;
    /// <summary>BoxCast result flag->True in the not hit frame</summary>
    public bool isExit { get; protected set; } = false;
    /// <summary>BoxCast result infos (warning! not hit frame = null)</summary>
    public BoxCastResult boxCastResult { get; private set; } = null;
	/// <summary>Raycast Center Position</summary>
	public Vector3 centerPosition { get; private set; } = Vector3.zero;

	public Vector3 direction { get { return GetBoxCastDirection(centerPosition); } }
	public LayerMaskEx layerMask { get { return m_raycastLayerMask; } }

	/// <summary>Direction of BoxCast</summary>
	[Header("BoxCast Info"), Tooltip("Direction of BoxCast"), SerializeField]
    BoxCastDirection m_direction = BoxCastDirection.Down;
	/// <summary>traget in target mode of raycast</summary>
	[Tooltip("Traget in target mode of raycast"), SerializeField]
	Transform m_target = null;
    /// <summary>Scale of BoxCast (transform.lossyScale * this)</summary>
    [Tooltip("Scale of BoxCast (transform.lossyScale * this)"), SerializeField]
    Vector3 m_scale = Vector3.one;
    /// <summary>Center of BoxCast</summary>
    [Tooltip("Center of BoxCast"), SerializeField]
    Vector3 m_center = Vector3.zero;
	/// <summary>Distance of BoxCast</summary>
	[Tooltip("Distance of BoxCast"), SerializeField]
    float m_distance = 1.0f;

    /// <summary>LayerMask of BoxCast</summary>
    [Header("Layer Mask Info"), Tooltip("LayerMask of BoxCast"), SerializeField]
    LayerMaskEx m_raycastLayerMask = int.MaxValue;

    //debug only
#if UNITY_EDITOR
    /// <summary>gizmo color -> hit frame</summary>
    static readonly Color m_cdHitColor = new Color(1.0f, 0.15f, 0.1f, 0.8f);
    /// <summary>gizmo color -> not hit frame</summary>
    static readonly Color m_cdNotHitColor = new Color(0.95f, 0.95f, 0.95f, 0.8f);

    /// <summary>Draw Gizmos? (debug only)</summary>
    [SerializeField, Tooltip("Draw Gizmos? (debug only)"), Header("Debug Only")]
    bool m_dIsDrawGizmos = false;
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

    /// <summary>
    /// [BoxCastDirection]
    /// return: m_directionに沿ったBoxCastの方向
    /// </summary>
    Vector3 GetBoxCastDirection(Vector3 center = new Vector3())
    {
        //intに変換
        int toBit = (int)m_direction;

        //各ビットでどの方向変数を使うか求めた後、-1 or 1で掛け算して返却
        if ((toBit & BoxCastDirectionBits.cForwardBit) != 0)
            return transform.forward * ((toBit & 0x2) - 1);
        else if ((toBit & BoxCastDirectionBits.cUpBit) != 0)
            return transform.up * ((toBit & 0x2) - 1);
        else if ((toBit & BoxCastDirectionBits.cRightBit) != 0)
            return transform.right * ((toBit & 0x2) - 1);
		else if ((toBit & BoxCastDirectionBits.cTarget) != 0)
			return (m_target.position - center).normalized;
		else
            return Vector3.zero;
    }

    /// <summary>[Update]</summary>
    void Update()
    {
        //初期化
        boxCastResult = null;
        
        //呼び出しコスト削減
        Transform myTransform = transform;
        Quaternion rotation = myTransform.rotation;
        Vector3 lossyScale = myTransform.lossyScale;
        Vector3 position = myTransform.position;

        //ボックスキャストのサイズ
        Vector3 boxCastScale = Vector3.Scale(lossyScale, m_scale) * 0.5f;
        //RaycastHit
        RaycastHit raycastHit = new RaycastHit();
        //フラグ計算に使用
        bool isOldStay = isStay;

		//center
		centerPosition = myTransform.LocalToWorldPosition(m_center);
		
		//Raycast
		isStay = Physics.BoxCast(centerPosition, boxCastScale, GetBoxCastDirection(centerPosition), 
			out raycastHit, rotation, m_distance, m_raycastLayerMask);

        //Hit!
        if (isStay)
        {
            //result代入
            boxCastResult = new BoxCastResult(raycastHit.transform, raycastHit.point, raycastHit.normal,
                1 << raycastHit.transform.gameObject.layer, raycastHit.distance, true);
        }
        //失敗した場合めりこみかどうか確認
        else
        {
			//全レイヤーマスクでCheckBox
			isStay = Physics.CheckBox(position, boxCastScale, rotation, m_raycastLayerMask);
			//Hitでresult代入
			if (isStay)
				boxCastResult = new BoxCastResult(raycastHit.transform, position, Vector3.zero, 0, 0.0f, false);
        }


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

        //debug only
#if UNITY_EDITOR
        //表示用変数更新
        m_dIsStayNow = isStay;
        m_dIsEnterNow = isEnter;
        m_dIsExitNow = isExit;
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
        Vector3 direction = GetBoxCastDirection(centerPosition);

        //Hit
        if (isStay)
        {
            //Rayの距離を求める
            float distance = (boxCastResult != null) ? boxCastResult.distance : m_distance;

            //Color
            Gizmos.color = m_cdHitColor;
            //Draw Ray
            Gizmos.DrawRay(centerPosition, direction * distance);
            //Matrix
            Gizmos.matrix = Matrix4x4.Translate(centerPosition + direction * distance);
            Gizmos.matrix *= Matrix4x4.Rotate(myTransform.rotation);
            //Draw Cube
            Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(myTransform.lossyScale, m_scale));
        }
        //Not Hit
        else
        {
            //Color
            Gizmos.color = m_cdNotHitColor;
            //Draw Ray
            Gizmos.DrawRay(centerPosition, direction * m_distance);
            //Matrix
            Gizmos.matrix = Matrix4x4.Translate(centerPosition + direction * m_distance);
            Gizmos.matrix *= Matrix4x4.Rotate(myTransform.rotation);
            //Draw Cube
            Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(myTransform.lossyScale, m_scale));
        }
    }
#endif
}