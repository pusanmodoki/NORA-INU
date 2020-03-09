//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE再生をオブジェクトで行うSEPlayer class
/// </summary>
public class SEPlayer : MonoBehaviour
{
	/// <summary>SeSources->Count</summary>
	public int numSources { get { return m_seSources.Count; } }

    /// <summary>
    /// 再生に必要な要素を定義したElement class
    /// </summary>
    [System.Serializable]
    public class Element
    {
        /// <summary>AudioSource</summary>
        [Tooltip("AudioSource")]
        public AudioSource source = null;
        /// <summary>SE Play Interval</summary>
        [Tooltip("SE Play Interval")]
        public float playInterval = 0.0f;
#if UNITY_EDITOR
        /// <summary>AudioClip Name (debug only)</summary>
        [Tooltip("AudioClip Name (debug only)")]
        public string dAudioClipName = "";
#endif
        /// <summary>SE Old Play Time</summary>
        [HideInInspector]
        public float playTime = 0.0f;
    }

	///<summary>再生用リスト</summary>
	[SerializeField, Tooltip("再生用リスト")]
    List<Element> m_seSources = new List<Element>();

	//Debug Only
#if UNITY_EDITOR
	///<summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
    {
		//Debug用Nameに設定
        foreach (var e in m_seSources)
        {
            if (e.source != null && e.source.clip != null)
                e.dAudioClipName = e.source.clip.name;
        }
    }
#endif

	/// <summary>
	/// [GetSource]
	/// return: AudioSource
	/// </summary>
	public AudioSource GetSource(int index)
	{
		return m_seSources[index].source;
	}

	/// <summary>
	/// [Stop]
	/// Loop再生中のSEを停止させる
	/// 引数1: index
	/// </summary>
	public void Stop(int index)
    {
        if (m_seSources.Count > index && index >= 0
			&& m_seSources[index].source.isPlaying)
            m_seSources[index].source.Stop();
    }
	/// <summary>
	/// [StopAll]
	/// Loop再生中のSEを全て停止させる
	/// </summary>
	public void StopAll()
    {
        foreach (var e in m_seSources)
            if (e.source.isPlaying) e.source.Stop();
    }

    /// <summary>
    /// [PlaySE]
    /// SEを再生する
    /// return: 再生可否
    /// 引数1: index
    /// 引数2: loop?
    /// </summary>
    public bool PlaySE(int index, bool isLoop = false)
    {
        //再生できるか
        if (m_seSources.Count > index && index >= 0
            && Time.time - m_seSources[index].playTime >= m_seSources[index].playInterval)
        {
            //値設定
            m_seSources[index].playTime = Time.time;
            m_seSources[index].source.loop = isLoop;
            //再生
            if (!isLoop) m_seSources[index].source.PlayOneShot(m_seSources[index].source.clip);
            else m_seSources[index].source.Play();

            return true;
        }
        return false;
    }
}
