using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-200)]
public class AudioManager : MonoBehaviour
{
	public enum FadeoutOption
	{
		None,
		AutoFadein,
		AutoStop
	}
	enum State
	{
		Null,
		Fadein,
		Fadeout,
		FadeoutChagne,
	}

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
	[System.Serializable]
	public struct VolumeInfo
	{
		public string uniqueKey { get { return m_uniqueKey; } }
		public float minVolume { get { return m_minVolume; } }
		public float changeSeconds { get { return m_changeSeconds; } }

		public VolumeInfo(string uniqueKey, float minVolume, float changeSeconds)
		{
			m_uniqueKey = uniqueKey;
			m_minVolume = minVolume;
			m_changeSeconds = changeSeconds;
		}

		public float ScalingMinVolume(float startVolume)
		{
			return startVolume * minVolume;
		}

		[SerializeField]
		string m_uniqueKey;
		[SerializeField, Range(0.0f, 1.0f)]
		float m_minVolume;
		[SerializeField, Range(0.1f, 1.0f)]
		float m_changeSeconds;
	}

	public static AudioManager instance { get; private set; } = null;
	static bool m_isCreateInstance = false;

	public ReadOnlyDictionary<string, AudioInfo> allBgms { get; private set; } = null;
	public ReadOnlyDictionary<string, LoadBgmInfo> bgmForEachScenes { get; private set; } = null;
	public ReadOnlyDictionary<string, VolumeInfo> allVolumeChangePresets { get; private set; } = null;
	public LoadBgmInfo bgmForNowScene { get { return m_bgmForEachScenesDictionary[OneMarkSceneManager.instance.nowLoadSceneName]; } }
	public bool isCompleteFadeChange { get; private set; } = false;

	[SerializeField]
	AudioSource m_bgmSource = null;
	[SerializeField]
	AudioSource m_freeAudioSource = null;
	[SerializeField]
	List<AudioInfo> m_allBgms = new List<AudioInfo>();
	[SerializeField]
	List<LoadBgmInfo> m_bgmForEachScenes = new List<LoadBgmInfo>();
	[SerializeField]
	List<VolumeInfo> m_allVolumeChangePresets = new List<VolumeInfo>();

	Dictionary<string, AudioInfo> m_allBgmsDictionary = new Dictionary<string, AudioInfo>();
	Dictionary<string, LoadBgmInfo> m_bgmForEachScenesDictionary = new Dictionary<string, LoadBgmInfo>();
	Dictionary<string, VolumeInfo> m_allVolumeChangePresetsDictionary = new Dictionary<string, VolumeInfo>();
	string m_playUniqueKey = null;
	string m_changeUniqueKey = null;
	string m_volumePresetUniqueKey = null;
	float m_startVolume = 0.0f;
	Timer m_timer = new Timer();
	State m_state = State.Null;
	FadeoutOption m_fadeoutOption = FadeoutOption.None;

	public void FreePlaySE(AudioSource source)
	{
		m_freeAudioSource.volume = source.volume;
		m_freeAudioSource.pitch = source.pitch;
		m_freeAudioSource.PlayOneShot(source.clip);
	}
	public void LoadAudios(string sceneName)
	{
		if (!bgmForEachScenes.ContainsKey(sceneName)) return;

		foreach(var bgmKey in bgmForEachScenes[sceneName].loadBgmKeys)
			allBgms[bgmKey].clip.LoadAudioData();
	}
	public bool WaitLoadAudios(string sceneName)
	{
		if (!bgmForEachScenes.ContainsKey(sceneName)) return false;

		foreach (var bgmKey in bgmForEachScenes[sceneName].loadBgmKeys)
		{
			if (allBgms[bgmKey].clip.loadState != AudioDataLoadState.Loaded)
				return false;
		}
		return true;
	}
	public void UnloadAudios(string sceneName)
	{
		if (!bgmForEachScenes.ContainsKey(sceneName)) return;

		m_playUniqueKey = null;
		foreach (var bgmKey in bgmForEachScenes[sceneName].loadBgmKeys)
			allBgms[bgmKey].clip.UnloadAudioData();
	}

	public void PlayBgm(string uniqueKey)
	{
		if (!m_allBgmsDictionary.ContainsKey(uniqueKey)) return;

		m_playUniqueKey = uniqueKey;
		m_bgmSource.clip = m_allBgmsDictionary[uniqueKey].clip;
		m_bgmSource.volume = m_allBgmsDictionary[uniqueKey].volume;
		m_bgmSource.loop = true;
		m_bgmSource.Play();
	}
	public void StopBgm(string uniqueKey)
	{
		if (!m_allBgmsDictionary.ContainsKey(uniqueKey)) return;

		m_playUniqueKey = null;
		m_bgmSource.Stop();
	}

	public bool FadeinBgm(string presetKey, bool isForceSetStartVolume = false)
	{
		if (m_playUniqueKey != null && m_allVolumeChangePresetsDictionary.ContainsKey(presetKey))
		{
			m_volumePresetUniqueKey = presetKey;
			m_state = State.Fadein;
			isCompleteFadeChange = false;
			if (isForceSetStartVolume)
				m_bgmSource.volume = m_allVolumeChangePresetsDictionary[m_volumePresetUniqueKey].minVolume;
			m_startVolume = m_bgmSource.volume;
			m_timer.Start();
			return true;
		}
		else
		{
#if UNITY_EDITOR
			Debug.Log("Error!! AudioManager->FadeinBgm bgm key:"+ m_playUniqueKey + "preset" + presetKey);
#endif
			return false;
		}
	}
	public bool FadeoutBgm(string presetKey, FadeoutOption fadeoutOption = FadeoutOption.None)
	{
		if (m_playUniqueKey != null && m_allVolumeChangePresetsDictionary.ContainsKey(presetKey))
		{
			m_volumePresetUniqueKey = presetKey;
			m_state = State.Fadeout;
			isCompleteFadeChange = false;
			m_fadeoutOption = fadeoutOption;
		//	m_bgmSource.volume = m_allBgmsDictionary[m_playUniqueKey].volume;
			m_startVolume = m_bgmSource.volume;
			m_timer.Start();
			return true;
		}
		else
		{
#if UNITY_EDITOR
			Debug.Log("Error!! AudioManager->FadeoutBgm key:" + m_playUniqueKey + "preset" + presetKey);
#endif
			return false;
		}
	}
	public bool FadeoutAndChangeBgm(string presetKey, string changeBgmUniqueKey)
	{
		if (m_playUniqueKey != null && m_allVolumeChangePresetsDictionary.ContainsKey(presetKey)
			&& m_allBgmsDictionary.ContainsKey(changeBgmUniqueKey))
		{
			m_volumePresetUniqueKey = presetKey;
			m_changeUniqueKey = changeBgmUniqueKey;
			isCompleteFadeChange = false;
			m_state = State.FadeoutChagne;
			//m_bgmSource.volume = m_allBgmsDictionary[m_playUniqueKey].volume;
			m_startVolume = m_bgmSource.volume;
			m_timer.Start();
			return true;
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! AudioManager->FadeoutBgm key:" + m_playUniqueKey + "preset" + presetKey);
#endif
			return false;
		}
	}


	void Awake()
	{
		if (!m_isCreateInstance)
		{
			m_isCreateInstance = true;
			instance = this;
			InitDictionary();
			DontDestroyOnLoad(gameObject);

			var listener = new GameObject("Listener");
			listener.AddComponent<AudioListener>();
			listener.transform.position = Vector3.zero;
			listener.transform.localScale = Vector3.zero;
			listener.transform.rotation = Quaternion.identity;
			DontDestroyOnLoad(listener);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	void Update()
	{
		float minVolume = m_state != State.Null ?  allVolumeChangePresets[m_volumePresetUniqueKey].ScalingMinVolume(m_startVolume) : 0.0f;

		switch (m_state)
		{
			case State.Null:
				return;
			case State.Fadeout:
				{
					if (m_timer.elapasedTime > allVolumeChangePresets[m_volumePresetUniqueKey].changeSeconds)
					{
						m_bgmSource.volume = minVolume;
						m_state = State.Null;
						isCompleteFadeChange = true;

						if (m_fadeoutOption == FadeoutOption.AutoFadein)
							FadeinBgm(m_volumePresetUniqueKey);
						else if (m_fadeoutOption == FadeoutOption.AutoStop)
							StopBgm(m_playUniqueKey);
					}
					else if (m_bgmSource.volume > minVolume)
					{
						m_bgmSource.volume = minVolume + (m_startVolume - minVolume)
							* (1.0f - (m_timer.elapasedTime / allVolumeChangePresets[m_volumePresetUniqueKey].changeSeconds));
					}
					break;
				}
			case State.Fadein:
				{
					if (m_timer.elapasedTime > allVolumeChangePresets[m_volumePresetUniqueKey].changeSeconds)
					{
						m_bgmSource.volume = allBgms[m_playUniqueKey].volume;
						m_state = State.Null;
						isCompleteFadeChange = true;
					}
					else if (m_bgmSource.volume < allBgms[m_playUniqueKey].volume)
					{
						m_bgmSource.volume = minVolume + (allBgms[m_playUniqueKey].volume - m_startVolume)
							* (m_timer.elapasedTime / allVolumeChangePresets[m_volumePresetUniqueKey].changeSeconds);
					}
					break;
				}
			case State.FadeoutChagne:
				{
					if (m_timer.elapasedTime > allVolumeChangePresets[m_volumePresetUniqueKey].changeSeconds)
					{
						m_bgmSource.volume = minVolume;
						m_state = State.Null;

						StopBgm(m_playUniqueKey);
						PlayBgm(m_changeUniqueKey);
						FadeinBgm(m_volumePresetUniqueKey);
					}
					else if (m_bgmSource.volume > minVolume)
					{
						m_bgmSource.volume = minVolume + (m_startVolume - minVolume)
							* (1.0f - (m_timer.elapasedTime / allVolumeChangePresets[m_volumePresetUniqueKey].changeSeconds));
					}
					break;
				}
			default:
				return;
		}
	}

	void InitDictionary()
	{
		allBgms = new ReadOnlyDictionary<string, AudioInfo>(m_allBgmsDictionary);
		bgmForEachScenes = new ReadOnlyDictionary<string, LoadBgmInfo>(m_bgmForEachScenesDictionary);
		allVolumeChangePresets = new ReadOnlyDictionary<string, VolumeInfo>(m_allVolumeChangePresetsDictionary);

		foreach (var element in m_allBgms)
			m_allBgmsDictionary.Add(element.uniqueKey, element);

		foreach (var element in m_bgmForEachScenes)
		{
			m_bgmForEachScenesDictionary.Add(element.sceneName, element);
			m_bgmForEachScenesDictionary[element.sceneName] 
				= new LoadBgmInfo(m_bgmForEachScenesDictionary[element.sceneName]);
		}

		foreach(var element in m_allVolumeChangePresets)
			m_allVolumeChangePresetsDictionary.Add(element.uniqueKey, element);
	}
}
