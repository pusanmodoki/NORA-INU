using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-200)]
public class AudioManager : MonoBehaviour
{
	[System.Serializable]
	public struct AudioInfo
	{
		public AudioInfo(string uniqueKey, AudioClip clip, float volume)
		{
			m_uniqueKey = uniqueKey;
			m_clip = clip;
			m_volume = volume;
		}

		public string uniqueKey { get { return m_uniqueKey; } }
		public AudioClip clip { get { return m_clip; } }
		public float volume { get { return m_volume; } }

		[SerializeField]
		string m_uniqueKey;
		[SerializeField]
		AudioClip m_clip;
		[SerializeField, Range(0.0f, 1.0f)]
		float m_volume;
	}
	[System.Serializable]
	public struct LoadBgmInfo
	{
		public LoadBgmInfo(LoadBgmInfo copy)
		{
			this.m_sceneName = copy.sceneName;
			this.m_loadBgmKeys = copy.m_loadBgmKeys;
			this.loadBgmKeys = new ReadOnlyCollection<string>(m_loadBgmKeys);
		}

		public LoadBgmInfo(string sceneName, string[] loadBgmKeys)
		{
			this.m_sceneName = sceneName;
			this.m_loadBgmKeys = loadBgmKeys;
			this.loadBgmKeys = new ReadOnlyCollection<string>(m_loadBgmKeys);
		}

		public string sceneName { get { return m_sceneName; } }
		public ReadOnlyCollection<string> loadBgmKeys { get; private set; }

		[SerializeField]
		string m_sceneName;
		[SerializeField]
		string[] m_loadBgmKeys;
	}

	public static AudioManager instance { get; private set; } = null;
	static bool m_isCreateInstance = false;

	public ReadOnlyDictionary<string, AudioInfo> allBgms { get; private set; } = null;
	public ReadOnlyDictionary<string, LoadBgmInfo> bgmForEachScenes { get; private set; } = null;
	public LoadBgmInfo bgmForNowScene { get { return m_bgmForEachScenesDictionary[OneMarkSceneManager.instance.nowLoadSceneName]; } }

	[SerializeField]
	AudioSource m_bgmSource = null;
	[SerializeField]
	List<AudioInfo> m_allBgms = new List<AudioInfo>();
	[SerializeField]
	List<LoadBgmInfo> m_bgmForEachScenes = new List<LoadBgmInfo>();

	Dictionary<string, AudioInfo> m_allBgmsDictionary = new Dictionary<string, AudioInfo>();
	Dictionary<string, LoadBgmInfo> m_bgmForEachScenesDictionary = new Dictionary<string, LoadBgmInfo>();

	public void LoadAudios(string sceneName)
	{
		foreach(var bgmKey in bgmForEachScenes[sceneName].loadBgmKeys)
			allBgms[bgmKey].clip.LoadAudioData();
	}
	public bool WaitLoadAudios(string sceneName)
	{
		foreach (var bgmKey in bgmForEachScenes[sceneName].loadBgmKeys)
		{
			if (allBgms[bgmKey].clip.loadState != AudioDataLoadState.Loaded)
				return false;
		}
		return true;
	}
	public void UnloadAudios(string sceneName)
	{
		foreach (var bgmKey in bgmForEachScenes[sceneName].loadBgmKeys)
			allBgms[bgmKey].clip.UnloadAudioData();
	}

	public void PlayBgm(string uniqueKey)
	{
		m_bgmSource.clip = m_allBgmsDictionary[uniqueKey].clip;
		m_bgmSource.volume = m_allBgmsDictionary[uniqueKey].volume;
		m_bgmSource.loop = true;
		m_bgmSource.Play();
	}
	public void StopBgm(string uniqueKey)
	{
		m_bgmSource.Stop();
	}

	void Awake()
	{
		if (!m_isCreateInstance)
		{
			m_isCreateInstance = true;
			instance = this;
			InitDictionary();
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void InitDictionary()
	{
		allBgms = new ReadOnlyDictionary<string, AudioInfo>(m_allBgmsDictionary);
		bgmForEachScenes = new ReadOnlyDictionary<string, LoadBgmInfo>(m_bgmForEachScenesDictionary);

		foreach (var element in m_allBgms)
			m_allBgmsDictionary.Add(element.uniqueKey, element);

		foreach (var element in m_bgmForEachScenes)
		{
			m_bgmForEachScenesDictionary.Add(element.sceneName, element);
			m_bgmForEachScenesDictionary[element.sceneName] 
				= new LoadBgmInfo(m_bgmForEachScenesDictionary[element.sceneName]);
		}
	}
}
