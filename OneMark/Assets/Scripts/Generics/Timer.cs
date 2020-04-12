//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定秒数経過ごとにtrueを返せるTimeSignal class
/// </summary>
public class TimeSignal
{
    /// <summary> Check Start Time</summary>
    public float startTime { get; protected set; }
    /// <summary> Check Signal Interval</summary>
    public float signalInterval { get; protected set; }
 
    /// <summary>now</summary>
    float m_nowLimitTime = 0.0f;
    
    /// <summary>
    /// [Start]
    /// 計測を開始する
    /// 引数1: Check関数がtrueを返す秒数間隔
    /// </summary>
    public void Start(float signaInterval)
    {
        startTime = Time.time;
        this.signalInterval = signaInterval;
        m_nowLimitTime = signaInterval;
    }

    /// <summary>
    /// [Start]
    /// return: singnalInterval秒経過したらtrueを返す
    /// </summary>
    public bool Check()
    {
        if (Time.time - startTime >= m_nowLimitTime)
        {
            startTime = Time.time;
            m_nowLimitTime = signalInterval + Time.time - startTime;
            return true;
        }
        else
            return false;
    }
}

/// <summary>
/// 経過秒数を測るTimer class
/// </summary>
public class Timer
{
	/// <summary>Measure Start Time</summary>
	public float startTime { get; protected set; } = 0.0f;
    /// <summary>Measure Elapased Time</summary>
    public float elapasedTime { get { return Time.time - startTime; } set { startTime = Time.time - value; } }
	/// <summary>Is Start?</summary>
	public bool isStart { get { return startTime > 0.0f; } }
	/// <summary>Is Stop?</summary>
	public bool isStop { get { return startTime < Mathf.Epsilon; } }

	/// <summary>
	/// [Start]
	/// 計測を開始する
	/// </summary>
	public void Start()
    {
        startTime = Time.time;
	}

	/// <summary>
	/// [Stop]
	/// 計測を停止する
	/// </summary>
	public void Stop()
	{
		startTime = 0;
	}
}

/// <summary>
/// Timerを拡張したTimerAdvance class
/// 計算処理はTimerと比べて若干遅い
/// </summary>
public class TimerAdvance
{
	/// <summary>Measure dtart time</summary>
	public float startTime { get; protected set; } = 0.0f;
	/// <summary>Measure elapased time</summary>
	public float elapasedTime {
		get
		{
			if (!isPause) return (Time.time - startTime + m_savedLastPauseElapased) * timeScale;
			else return m_savedLastPauseElapased * timeScale;
		}
		set
		{
			startTime = Time.time - value;
			m_savedLastPauseElapased = 0.0f;
		}
	}
	/// <summary>Manual time dcale</summary>
	public float timeScale { get; set; } = 1.0f;
	/// <summary>Is dtart?</summary>
	public bool isStart { get; private set; } = false;
	/// <summary>Is stop?</summary>
	public bool isStop { get { return !isStart; } }
	/// <summary>Is pause?</summary>
	public bool isPause { get; private set; } = false;

	/// <summary>ポーズした際に加算されるそれまでの経過時間</summary>
	float m_savedLastPauseElapased = 0.0f;

	/// <summary>
	/// [Start]
	/// 計測を開始する
	/// </summary>
	public void Start()
	{
		startTime = Time.time;
		isStart = true;
	}
	/// <summary>
	/// [Stop]
	/// 計測を停止する
	/// </summary>
	public void Stop()
	{
		startTime = 0.0f;
		m_savedLastPauseElapased = 0.0f;
		isStart = false;
	}

	/// <summary>
	/// [Pause]
	/// 計測を一時停止する
	/// </summary>
	public void Pause()
	{
		if (isStop | isPause) return;
		m_savedLastPauseElapased += (Time.time - startTime);
		isPause = true;
	}
	/// <summary>
	/// [Unpause]
	/// 計測を再開する
	/// </summary>
	public void Unpause()
	{
		if (isStop | !isPause) return;
		startTime = Time.time;
		isPause = false;
	}
}