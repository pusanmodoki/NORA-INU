//制作者: 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 神風特攻を指示, 情報を保存するKamikazeCommand 
/// </summary>
public class KamikazeCommand : MonoBehaviour
{
	/// <summary>Kamikaze direction</summary>
	public Vector3 direction { get; private set; } = Vector3.zero;
	/// <summary>Kamikaze timeout seconds</summary>
	public float timeoutSeconds { get; private set; } = 0.0f;
	/// <summary>Kamikaze now?</summary>
	public bool isKamikazeNow { get; private set; } = false;

	/// <summary>This ai agent</summary>
	[SerializeField, Tooltip("This ai agent")]
	AIComponent.AIAgent m_aiAgent = null;
	/// <summary>This kamikaze function</summary>
	[SerializeField, Tooltip("This kamikaze function")]
	AIComponent.KamikazeMove m_kamikazeFunction = null;
	[SerializeField, Tooltip("This speed changer")]
	SpeedChanger m_speedChanger = null;

	/// <summary>
	/// [InvokeCommand]
	/// 神風特攻を指示する
	/// 引数1: 方向 (world)
	/// 引数2: 神風特攻をする最長時間
	/// </summary>
	public void InvokeCommand(Vector3 direction, float timeoutSeconds, float setAcceleration)
	{
		isKamikazeNow = true;
		this.direction = direction;
		this.timeoutSeconds = timeoutSeconds;


		m_speedChanger.ForceSetAcceleration(setAcceleration);
		m_aiAgent.ForceSpecifyFunction(m_kamikazeFunction);
	}

	/// <summary>
	/// [EndKamikaze]
	/// 神風特攻を終了する
	/// </summary>
	public void EndKamikaze()
	{
		isKamikazeNow = false;
	}
}
