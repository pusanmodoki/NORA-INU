using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Dogのスピードを調整するDogSpeedChanger
/// </summary>
[System.Serializable]
public class DogSpeedChanger
{
	/// <summary>基準速度</summary>
	public float targetSpeed { get { return m_targetSpeed; } }
	/// <summary>勾配モード</summary>
	public bool isGradientMode { get; private set; } = false;

	/// <summary>This NavMeshAgent</summary>
	[Header("Reference"), SerializeField, Tooltip("This NavMeshAgent")]
	NavMeshAgent m_navMeshAgent = null;
	/// <summary>This BoxCastFlags (ground flag)</summary>
	[SerializeField, Tooltip("This BoxCastFlags (ground flag)")]
	BoxCastFlags m_groundFlags = null;
	/// <summary>This BoxCastFlags (gradient flag)</summary>
	[SerializeField, Tooltip("This BoxCastFlags (gradient flag)")]
	BoxCastFlags m_gradientFlags = null;

	/// <summary>基準速度</summary>
	[Header("Speed of NavMesh"), SerializeField, Tooltip("基準速度")]
	float m_targetSpeed = 1.0f;
	/// <summary>最小速度</summary>
	[SerializeField, Tooltip("最小速度")]
	float m_minSpeed = 0.1f;
	/// <summary>最大速度</summary>
	[SerializeField, Tooltip("最大速度")]
	float m_maxSpeed = 20.0f;
	/// <summary>マニュアル設定加速度の減速速度</summary>
	[SerializeField, Range(0.0f, 10.0f), Tooltip("マニュアル設定加速度の減速速度")]
	float m_manualDecelerationSeconds = 0.1f;

	/// <summary>下り坂加速度</summary>
	[Header("Gradient acceleration per seconds"), SerializeField, Tooltip("下り坂加速度")]
	float m_gradientAcceleration = 1.0f;
	/// <summary>勾配による減速倍率(上り坂に使う)</summary>
	[SerializeField, Range(0.0f, 1.0f), Tooltip("勾配による減速倍率(上り坂に使う)")]
	float m_gradientDecelerationRatio = 0.5f;
	/// <summary>下り坂最大加速度</summary>
	[SerializeField, Tooltip("下り坂最大加速度")]
	float m_gradientMaxAcceleration = 10.0f;
	/// <summary>下り坂で加速後平面で減速する際に何秒かかるか</summary>
	[SerializeField, Range(0.0f, 10.0f), Tooltip("下り坂で加速後平面で減速する際に何秒かかるか")]
	float m_gradientDecelerationSeconds = 0.1f;

	/// <summary>勾配モードと判断する秒数</summary>
	[Header("Other"), SerializeField, Range(0.0f, 0.1f), Tooltip("勾配モードと判断する秒数")]
	float m_gradientModeSeconds = 0.1f;

	//勾配モードタイマー
	Timer m_gradientModeTimer = new Timer();
	//勾配モードの現在加速度
	float m_gradientNowAcceleration = 0.0f;
	//マニュアル設定の現在加速度
	float m_manualNowAcceleration = 0.0f;

	/// <summary>
	/// [SetManualAcceleration]
	/// マニュアル設定の加速度を設定する
	/// 引数1: 加速度
	/// </summary>
	public void SetManualAcceleration(float acceleration)
	{
		m_manualNowAcceleration = acceleration;
	}

	/// <summary>[Start]</summary>
	public void Start()
	{
		//nullチェック
#if UNITY_EDITOR
		if (m_navMeshAgent == null)
			Debug.LogError("Error!! SpeedChanger->Awake NavMeshAgent == null");
		if (m_groundFlags == null)
			Debug.LogError("Error!! SpeedChanger->Awake GroundFlags == null");
		if (m_gradientFlags == null)
			Debug.LogError("Error!! SpeedChanger->Awake GradientFlags == null");
#endif
	}
	/// <summary>[Update]</summary>
	public void Update()
	{
		//接地してなければ終了
		if (!m_groundFlags.isStay) return;

		//勾配率を確認
		float dotGradient = Vector3.Dot(m_groundFlags.boxCastResult.normal, Vector3.up);
		//勾配モードにするか判断
		CheckGradient(dotGradient);

		//スピード決定, 代入
		m_navMeshAgent.speed = CalculateSpeed(dotGradient);
	}


	/// <summary>
	/// [CheckGradient]
	/// 勾配モードにするか判断する
	/// 引数1: 勾配率
	/// </summary>
	void CheckGradient(float dotGradient)
	{
		//傾斜かつ指定秒数経過ならStart
		if (dotGradient < 1.0f - Mathf.Epsilon)
		{
			if (!m_gradientModeTimer.isStart)
				m_gradientModeTimer.Start();

			if (m_gradientModeTimer.elapasedTime > m_gradientModeSeconds)
				isGradientMode = true;
		}
		else
		{
			m_gradientModeTimer.Stop();
			isGradientMode = false;
		}
	}
	/// <summary>
	/// [CalculateSpeed]
	/// 速度を決定する
	/// 引数1: 勾配率
	/// </summary>
	float CalculateSpeed(float dotGradient)
	{
		//マニュアルの加速度を設定
		m_manualNowAcceleration -= (m_manualNowAcceleration / m_manualDecelerationSeconds) * Time.deltaTime;
		if (m_manualNowAcceleration < 0.001f)
			m_manualNowAcceleration = 0.0f;

		//勾配でない場合
		if (!isGradientMode)
		{
			//勾配用加速度を減速
			m_gradientNowAcceleration -= 
				(m_gradientNowAcceleration / m_gradientDecelerationSeconds) * Time.deltaTime;
			if (m_gradientNowAcceleration < 0.001f)
				m_gradientNowAcceleration = 0.0f;

			//速度決定
			return Mathf.Clamp(m_targetSpeed +
				m_manualNowAcceleration + m_gradientNowAcceleration, m_minSpeed, m_maxSpeed);
		}

		//勾配(下り坂)
		if (!m_gradientFlags.isStay)
		{
			//勾配用加速度を加算
			m_gradientNowAcceleration += m_gradientAcceleration * Time.deltaTime;
			if (m_gradientNowAcceleration > m_gradientMaxAcceleration)
				m_gradientNowAcceleration = m_gradientMaxAcceleration;

			//速度決定
			return Mathf.Clamp(m_targetSpeed + m_manualNowAcceleration + m_gradientNowAcceleration, m_minSpeed, m_maxSpeed);
		}
		else
		{
			//勾配用加速度を減速
			m_gradientNowAcceleration -= (m_gradientNowAcceleration / m_gradientDecelerationSeconds) * Time.deltaTime;
			if (m_gradientNowAcceleration < 0.001f)
				m_gradientNowAcceleration = 0.0f;

			//速度決定
			return Mathf.Clamp(dotGradient * m_gradientDecelerationRatio * m_targetSpeed
				+ m_manualNowAcceleration + m_gradientNowAcceleration, m_minSpeed, m_maxSpeed);
		}
	}
}
