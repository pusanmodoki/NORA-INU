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
    public float startTime { get; protected set; }
    /// <summary>Measure Elapased Time</summary>
    public float elapasedTime { get { return Time.time - startTime; } set { startTime = Time.time - value; } }

    /// <summary>
    /// [Start]
    /// 計測を開始する
    /// </summary>
    public void Start()
    {
        startTime = Time.time;
    }
}



/// <summary>
/// 経過秒数を測るTimerScaling class (timeScale対応)
/// </summary>
public class TimerScaling
{
    /// <summary>Measure Elapased Time</summary>
    public float elapasedTime { get; protected set; } = 0.0f;
    /// <summary>Measure Time Scale</summary>
    public float timeScale { get; set; } = 1.0f;

    /// <summary>
    /// [Start]
    /// 計測を開始する
    /// </summary>
    public void Start()
    {
        elapasedTime = 0.0f;
    }

    /// <summary>
    /// [Update]
    /// 計測更新
    /// </summary>
    public void Update()
    {
        elapasedTime += Time.deltaTime * timeScale;
    }
}