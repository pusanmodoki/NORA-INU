using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-180)]
public class OneMarkSceneManager : MonoBehaviour
{
	public enum SceneState
	{
		Title,
		StageSelect,
		Stage
	}

	public static ReadOnlyCollection<string> cStageSceneNames = new ReadOnlyCollection<string>
	(
		new string[]
		{
			"T-1", "T-2", "T-3", "T-4",
			"1-1", "1-2", "1-3", "1-4",
			"2-1", "2-2", "2-3", "2-4",
			"3-1", "3-2", "3-3", "3-4",
			"4-1", "4-2", "4-3", "4-4",
		}
	);
	public static ReadOnlyCollection<Vector2Int> cStageSceneIndexes = new ReadOnlyCollection<Vector2Int>
	(
		new Vector2Int[]
		{
			new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4),
			new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3), new Vector2Int(1, 4),
			new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3), new Vector2Int(2, 4),
			new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(3, 4),
		}
	);
	public static Vector2Int cInvalidStageIndex = new Vector2Int(-1, -1);

	public static OneMarkSceneManager instance { get; private set; } = null;
	static bool m_isCreateInstance = false;

	public Scene nowScene { get { return SceneManager.GetSceneByName(nowLoadSceneName); } }
	public Vector2Int nowStageSceneIndex
	{
		get
		{
			Vector2Int result = Vector2Int.zero;
			int index = -1;

			if (int.TryParse(nowLoadSceneName[0].ToString(), out index)) result.x = index;
			else if (nowLoadSceneName[0] != 'T') return cInvalidStageIndex;

			if (int.TryParse(nowLoadSceneName[2].ToString(), out index)) result.y = index;
			else return cInvalidStageIndex;

			return cStageSceneIndexes.Contains(result) ? result : cInvalidStageIndex;
		}
	}
	public Vector2Int nextStageSceneIndex
	{
		get
		{
			Vector2Int result = nowStageSceneIndex;

			if (result == cInvalidStageIndex 
				|| (result == cStageSceneIndexes[cStageSceneIndexes.Count - 1])) return cInvalidStageIndex;

			if (++result.y > cStageSceneIndexes[cStageSceneIndexes.Count - 1].y)
			{
				result.y = 1;

				if (++result.x > cStageSceneIndexes[cStageSceneIndexes.Count - 1].x)
					result.x = 0;
			}

			return result;
		}
	}
	public bool isNowStageScene
	{
		get
		{
			int parse = 0;
			bool isResult = true;
			isResult &= ((nowLoadSceneName[0] == 'T') || (int.TryParse(nowLoadSceneName[0].ToString(), out parse)));
			isResult &= (nowLoadSceneName[1] == '-');
			isResult &= (int.TryParse(nowLoadSceneName[2].ToString(), out parse));

			return isResult;
		}
	}
	public string nowLoadSceneName { get; private set; } = "";
	public float loadProgress { get; private set; } = 0.0f;
	public bool isNowFinalStage { get { return m_nowScene.isLoaded && m_nowScene.name == "4-4"; } }
	public bool isCompleteFadeOut { get { return m_fadeScreen.isCompletedTransition && m_fadeScreen.fadeState == FadeScreen.FadeState.Fadeout; } }
	public bool isActiveOption { get; private set; } = false;

	[Header("Scene Transition"), SerializeField]
	SceneName m_gameStartSceneName = default;
	[SerializeField]
	SceneName m_logoSceneName = default;
	[SerializeField]
	SceneName m_titleSceneName = default;
	[SerializeField]
	SceneName m_stageSelectSceneName = default;
	//[SerializeField]
	//SceneName m_optionSceneName = default;
	[SerializeField]
	SceneName m_loadScenename = default;
	[SerializeField]
	SceneNameArray m_accessoriesForStageScenes = default;
	[SerializeField]
	float m_minTimeToLoad = 1.0f;

	[Header("Fade In And Out"), SerializeField]
	FadeScreen m_fadeScreen = null;
	[SerializeField]
	Color m_fadeColor = Color.black;
	[SerializeField]
	float m_fadeSpeedPerSeconds = 1.0f;

	Dictionary<string, Scene> m_accessoryScenes = null;
	Scene m_nowScene = default;
	Scene m_optionScene = default;
//	AsyncOperation m_optionOperation = null;

	public void MoveScene(SceneState sceneState)
	{
		switch(sceneState)
		{
			case SceneState.Title:
				{
					StartCoroutine(LoadScene(m_titleSceneName, false));
					break;
				}
			case SceneState.StageSelect:
				{
					StartCoroutine(LoadScene(m_stageSelectSceneName, false));
					break;
				}
		}
	}
	public void ReloadScene()
	{
		if (nowLoadSceneName == m_titleSceneName
			|| nowLoadSceneName == m_stageSelectSceneName)
		{
			StartCoroutine(LoadScene(nowLoadSceneName, false));
		}
		else
		{
			StartCoroutine(LoadScene(nowLoadSceneName, true));
		}
	}
	public void MoveStageScene(Vector2Int sceneIndex)
	{
		if (!cStageSceneIndexes.Contains(sceneIndex)) return;

		StartCoroutine(LoadScene(sceneIndex.ToStageName(), true));
	}
	public void MoveStageScene(int sceneIndex)
	{
		if (cStageSceneIndexes.Count <= sceneIndex || sceneIndex < 0) return;

		StartCoroutine(LoadScene(cStageSceneIndexes[sceneIndex].ToStageName(), true));
	}
	public bool MoveNextStage()
	{
		Vector2Int index = nextStageSceneIndex;
		if (index == cInvalidStageIndex) return false;

		string stageName = index.ToStageName();
		StartCoroutine(LoadScene(stageName, true));

		return true;
	}
	public void SetActiveAccessoryScene(string sceneName, bool isSet)
	{

		if (!m_accessoryScenes.ContainsKey(sceneName) || !m_accessoryScenes[sceneName].IsValid())
			return;
		//	if ((!m_accessoryScenes.ContainsKey(sceneName) || !m_accessoryScenes[sceneName].IsValid())
		//		&& (m_optionSceneName == sceneName && m_optionScene.IsValid()))
		//		return;

		//if (m_optionSceneName == sceneName)
		//{
		//	var gameObjects = m_optionScene.GetRootGameObjects();
		//	for (int i = 0, length = gameObjects.Length; i < length; ++i)
		//		gameObjects[i].SetActive(isSet);
		//}
		//else
		{
			var gameObjects = m_accessoryScenes[sceneName].GetRootGameObjects();
			for (int i = 0, length = gameObjects.Length; i < length; ++i)
				gameObjects[i].SetActive(isSet);
		}
	}
	public void SetActiveOptionScene(bool isSet)
	{
		if (m_optionScene.IsValid() == false || m_optionScene.isLoaded == false)
			return;

		isActiveOption = isSet;
		var gameObjects = m_optionScene.GetRootGameObjects();
		for (int i = 0, length = gameObjects.Length; i < length; ++i)
			gameObjects[i].SetActive(isSet);
	}

	void Awake()
	{
		if (!m_isCreateInstance)
		{
			m_isCreateInstance = true;
			instance = this;
			nowLoadSceneName = SceneManager.GetActiveScene().name;
			DontDestroyOnLoad(gameObject);
			Init();

			if (SceneManager.GetSceneByName(m_gameStartSceneName).IsValid())
				StartCoroutine(LoadScene(m_logoSceneName, false, true));
			else if (isNowStageScene)
			{
				StartCoroutine(LoadSceneOptionAndAccessory(true));

				AudioManager.instance.LoadAudios(nowLoadSceneName);
				if (AudioManager.instance.bgmForEachScenes.ContainsKey(nowLoadSceneName)
					&& AudioManager.instance.bgmForNowScene.loadBgmKeys.Count > 0)
				{
					AudioManager.instance.PlayBgm(AudioManager.instance.bgmForNowScene.loadBgmKeys[0]);
					AudioManager.instance.FadeinBgm("SceneChange", true);
				}
			}
			else
			{
				StartCoroutine(LoadSceneOptionAndAccessory(false));

				AudioManager.instance.LoadAudios(nowLoadSceneName);
				AudioManager.instance.WaitLoadAudios(nowLoadSceneName);
				if (AudioManager.instance.bgmForNowScene.loadBgmKeys.Count > 0)
				{
					AudioManager.instance.PlayBgm(AudioManager.instance.bgmForNowScene.loadBgmKeys[0]);
					AudioManager.instance.FadeinBgm("SceneChange", true);
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Init()
	{
		m_accessoryScenes = new Dictionary<string, Scene>();
		for (int i = 0; i < m_accessoriesForStageScenes.length; ++i)
			m_accessoryScenes.Add(m_accessoriesForStageScenes[i], default);

		SceneManager.sceneLoaded += SceneLoaded;
	}

	void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == LoadSceneMode.Additive)
		{
			var gameObjects = scene.GetRootGameObjects();
			foreach (var e in gameObjects) e.SetActive(false);
		}
	}

	IEnumerator LoadScene(string sceneName, bool isLoadAccessory, bool isFirstLoad = false)
	{
		if (!isFirstLoad)
		{
			m_fadeScreen.OnFadeScreen(m_fadeColor, m_fadeSpeedPerSeconds, FadeScreen.FadeState.Fadein, false);
			while (!m_fadeScreen.isCompletedTransition) yield return null;
		}
		else
			m_fadeScreen.SetFadeIn(m_fadeColor);

		var stopwatch = new System.Diagnostics.Stopwatch();
		loadProgress = 0.0f;
		m_nowScene = default;
		isActiveOption = false;
		stopwatch.Start();

		yield return UnloadScenes();
		nowLoadSceneName = sceneName;

		{
			AsyncOperation operation = SceneManager.LoadSceneAsync(m_loadScenename, LoadSceneMode.Single);
			while (!operation.isDone) yield return null;
			while (true)
			{
				var loadScene = SceneManager.GetActiveScene();
				if (loadScene.IsValid() && loadScene.isLoaded && loadScene.name == m_loadScenename) break;
				else yield return null;
			}
		}

		yield return LoadSceneOptionAndAccessory(isLoadAccessory);

		{
			AudioManager.instance.LoadAudios(sceneName);
			AsyncOperation mainSceneOperation = SceneManager.LoadSceneAsync(
				sceneName, LoadSceneMode.Additive);
			mainSceneOperation.allowSceneActivation = false;

			while (true)
			{
				loadProgress = mainSceneOperation.progress;

				if (mainSceneOperation.progress >= 0.9f) break;
				else yield return null;
			}

			AudioManager.instance.WaitLoadAudios(sceneName);

			mainSceneOperation.allowSceneActivation = true;
			while (true)
			{
				var mainScene = SceneManager.GetSceneByName(sceneName);
				if (mainScene.IsValid() && mainScene.isLoaded && mainScene.name == sceneName)
				{
					m_nowScene = mainScene;
					break;
				}
				else yield return null;
			}
		}

		{
			stopwatch.Stop();
			float elapased = (stopwatch.ElapsedMilliseconds * 0.001f);
			if (elapased < m_minTimeToLoad) yield return new WaitForSeconds(m_minTimeToLoad - elapased);
		}

		{
			AsyncOperation operation = SceneManager.UnloadSceneAsync(m_loadScenename);
			while (!operation.isDone) yield return null;
		}

		{
			SceneManager.SetActiveScene(m_nowScene);
			yield return null;
			var gameObjects = m_nowScene.GetRootGameObjects();
			foreach (var e in gameObjects) e.SetActive(true);
		}

		if (AudioManager.instance.bgmForNowScene.loadBgmKeys.Count > 0)
		{
			AudioManager.instance.PlayBgm(AudioManager.instance.bgmForNowScene.loadBgmKeys[0]);
			AudioManager.instance.FadeinBgm("SceneChange", true);
		}
		m_fadeScreen.OnFadeScreen(m_fadeColor, m_fadeSpeedPerSeconds, FadeScreen.FadeState.Fadeout, true);
	}

	IEnumerator LoadSceneOptionAndAccessory(bool isLoadAccessory)
	{
		float eachProgress = isLoadAccessory ? (0.1f / (m_accessoryScenes.Count + 1)) : 0.1f;

		//{
		//	AsyncOperation optionOperation = null;
		//	AudioManager.instance.LoadAudios(m_optionSceneName);
		//	yield return LoadOptionScene(optionOperation, eachProgress);

		//	AudioManager.instance.WaitLoadAudios(m_optionSceneName);
		//	m_optionOperation.allowSceneActivation = true;
		//	yield return null;
		//	while (true)
		//	{
		//		var optionScene = SceneManager.GetSceneByName(m_optionSceneName);
		//		if (optionScene.IsValid() && optionScene.isLoaded && optionScene.name == m_optionSceneName)
		//		{
		//			m_optionScene = optionScene;
		//			break;
		//		}
		//		else yield return null;
		//	}
		//}

		if (isLoadAccessory)
		{
			List<AsyncOperation> operations = new List<AsyncOperation>();
			float addProgress = eachProgress;

			for (int i = 0; i < m_accessoryScenes.Count; ++i, addProgress += eachProgress)
			{
				operations.Add(SceneManager.LoadSceneAsync(m_accessoriesForStageScenes[i], LoadSceneMode.Additive));
				AudioManager.instance.LoadAudios(m_accessoriesForStageScenes[i]);

				operations[i].allowSceneActivation = false;

				while (true)
				{
					loadProgress = 0.9f + addProgress + (eachProgress / (operations[i].progress / 0.9f));

					if (operations[i].progress >= 0.9f) break;
					else yield return null;
				}

				AudioManager.instance.WaitLoadAudios(m_accessoriesForStageScenes[i]);
				operations[i].allowSceneActivation = true;
				yield return null;
				while (true)
				{
					var accessoryScene = SceneManager.GetSceneByName(m_accessoriesForStageScenes[i]);
					if (accessoryScene.IsValid() && accessoryScene.isLoaded && accessoryScene.name == m_accessoriesForStageScenes[i])
					{
						m_accessoryScenes[accessoryScene.name] = accessoryScene;
						break;
					}
					else yield return null;
				}
			}
		}
	}
	//IEnumerator LoadOptionScene(AsyncOperation operation, float thisProgress)
	//{
	//	float progress = loadProgress;
	//	m_optionOperation = SceneManager.LoadSceneAsync(m_optionSceneName, LoadSceneMode.Additive);
	//	m_optionOperation.allowSceneActivation = false;

	//	while (true)
	//	{
	//		loadProgress = progress + (thisProgress / (m_optionOperation.progress / 0.9f));

	//		if (m_optionOperation.progress < 0.9f) yield return null;
	//		else yield break;
	//	}
	//}
	IEnumerator UnloadScenes()
	{
		bool isResult = AudioManager.instance.FadeoutBgm("SceneChange", AudioManager.FadeoutOption.AutoStop);
		while (isResult && !AudioManager.instance.isCompleteFadeChange) yield return null;

		foreach (var key in m_accessoriesForStageScenes.sceneNames)
		{
			if (m_accessoryScenes[key].IsValid())
			{
				yield return UnloadScene(m_accessoryScenes[key].name);
				AudioManager.instance.UnloadAudios(key);
				m_accessoryScenes[key] = default;
			}
		}

		//if (m_optionScene.IsValid())
		//{
		//	yield return UnloadScene(m_optionSceneName);
		//	m_optionScene = default;
		//}

		if (nowLoadSceneName != "")
			AudioManager.instance.UnloadAudios(nowLoadSceneName);
	}
	IEnumerator UnloadScene(string sceneName)
	{
		AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

		while (!operation.isDone) yield return null;

		yield break;
	}
}
