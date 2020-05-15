using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

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
			new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3),
			new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3),
			new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3),
			new Vector2Int(3, 0), new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3),
		}
	);
	public static Vector2Int cInvalidStageIndex = new Vector2Int(-1, -1);

	public static OneMarkSceneManager instance { get; private set; } = null;
	static bool m_isCreateInstance = false;

	public Vector2Int nowStageSceneIndex
	{
		get
		{
			if (!m_nowScene.isLoaded) return cInvalidStageIndex;

			Vector2Int result = Vector2Int.zero;
			int index = -1;

			if (int.TryParse(m_nowScene.name[0].ToString(), out index)) result.x = index;
			else if (m_nowScene.name[0] != 'T') return cInvalidStageIndex;

			if (int.TryParse(m_nowScene.name[2].ToString(), out index)) result.y = index;
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
				result.y = 0;

			if (++result.x > cStageSceneIndexes[cStageSceneIndexes.Count - 1].x)
				result.x = 0;

			return result;
		}
	}
	public bool isNowStageScene
	{
		get
		{
			if (!m_nowScene.isLoaded) return false;

			int parse = 0;
			bool isResult = true;
			isResult &= ((m_nowScene.name[0] == 'T') || (int.TryParse(m_nowScene.name[0].ToString(), out parse)));
			isResult &= (m_nowScene.name[1] == '-');
			isResult &= (int.TryParse(m_nowScene.name[2].ToString(), out parse));

			return isResult;
		}
	}
	public float loadProgress { get; private set; } = 0.0f;
	public bool isNowFinalStage { get { return m_nowScene.isLoaded && m_nowScene.name == "4-4"; } }

	[SerializeField]
	SceneName m_selectTitleScene = default;
	[SerializeField]
	SceneName m_selectStageSelectScene = default;
	[SerializeField]
	SceneName m_selectOptionScene = default;
	[SerializeField]
	SceneName m_selectLoadScene = default;
	[SerializeField]
	SceneNameArray m_accessoriesForStageScenes = default;

	Dictionary<string, Scene> m_accessoryScenes = null;
	Scene m_nowScene = default;
	Scene m_optionScene = default;

	public void MoveScene(SceneState sceneState)
	{

	}
	public void MoveScene(SceneState sceneState, Vector2Int sceneIndex)
	{

	}
	public void MoveScene(SceneState sceneState, int sceneIndex)
	{

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
		if (!m_accessoryScenes.ContainsKey(sceneName) 
			|| !m_accessoryScenes[sceneName].IsValid())
			return;

		var gameObjects = m_accessoryScenes[sceneName].GetRootGameObjects();
		for (int i = 0, length = gameObjects.Length; i < length; ++i)
			gameObjects[i].SetActive(isSet);
	}
	public void SetActiveOptionScene(bool isSet)
	{
		if (m_optionScene.IsValid() == false || m_optionScene.isLoaded == false)
			return;

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
			Init();
			DontDestroyOnLoad(gameObject);
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

		SceneManager.LoadScene(m_selectOptionScene.sceneName, LoadSceneMode.Additive);
	}

	IEnumerator LoadScene(string sceneName, bool isLoadAccessory)
	{
		AsyncOperation mainSceneOperation = null;

		loadProgress = 0.0f;
		m_nowScene = default;

		yield return UnloadScenes();

		{
			AsyncOperation operation = SceneManager.LoadSceneAsync(
				m_selectLoadScene.sceneName, LoadSceneMode.Single);
			while (!operation.isDone) yield return null;
		}

		mainSceneOperation = SceneManager.LoadSceneAsync(
			sceneName, LoadSceneMode.Single);
		mainSceneOperation.allowSceneActivation = false;

		while (true)
		{
			loadProgress = mainSceneOperation.progress;

			if (mainSceneOperation.progress >= 0.9f) break;
			else yield return null;
		}

		if (!isLoadAccessory)
		{
			AsyncOperation optionOperation = null;
			LoadOptionScene(optionOperation, 0.1f);

			mainSceneOperation.allowSceneActivation = true;
			while (true)
			{
				var scene = SceneManager.GetActiveScene();
				if (scene.IsValid() && scene.isLoaded && scene.name == sceneName)
				{
					m_nowScene = scene;
					break;
				}
				else yield return null;
			}

			optionOperation.allowSceneActivation = true;
			while (true)
			{
				var scene = SceneManager.GetSceneByName(m_selectLoadScene.sceneName);
				if (scene.IsValid() && scene.isLoaded)
				{
					m_optionScene = scene;
					yield break;
				}
				else yield return null;
			}
		}
		else
		{
			List<AsyncOperation> operations = new List<AsyncOperation>();
			AsyncOperation optionOperation = null;
			float eachProgress = 0.1f / (m_accessoryScenes.Count + 1);
			float addProgress = eachProgress;

			LoadOptionScene(optionOperation, eachProgress);

			for (int i = 0; i < m_accessoryScenes.Count; ++i, addProgress += eachProgress)
			{
				operations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single));
				operations[i].allowSceneActivation = false;

				while (true)
				{
					loadProgress = 0.9f + addProgress + (operations[i].progress / eachProgress);

					if (operations[i].progress >= 0.9f) break;
					else yield return null;
				}
			}

			mainSceneOperation.allowSceneActivation = true;
			while (true)
			{
				var scene = SceneManager.GetActiveScene();
				if (scene.IsValid() && scene.isLoaded && scene.name == sceneName)
				{
					m_nowScene = scene;
					break;
				}
				else yield return null;
			}

			optionOperation.allowSceneActivation = true;
			while (true)
			{
				var scene = SceneManager.GetSceneByName(m_selectLoadScene.sceneName);
				if (scene.IsValid() && scene.isLoaded)
				{
					m_optionScene = scene;
					break;
				}
				else yield return null;
			}

			for (int i = 0, count = operations.Count; i < count; ++i)
				operations[i].allowSceneActivation = true;

			for (int i = 0, count = m_accessoryScenes.Count; i < count; ++i)
			{
				while (true)
				{
					var scene = SceneManager.GetSceneByName(m_accessoriesForStageScenes[i]);
					if (scene.IsValid() && scene.isLoaded)
					{
						m_accessoryScenes[scene.name] = scene;
						break;
					}
					else yield return null;
				}
			}

			yield break;
		}
	}
	IEnumerator LoadOptionScene(AsyncOperation operation, float thisProgress)
	{
		float progress = loadProgress;
		operation = SceneManager.LoadSceneAsync(
			m_selectOptionScene.sceneName, LoadSceneMode.Additive);
		operation.allowSceneActivation = false;

		while (true)
		{
			loadProgress = progress + ((operation.progress / 0.9f) / thisProgress);

			if (operation.progress < 0.9f) yield return null;
			else yield break;
		}
	}
	IEnumerator UnloadScenes()
	{
		foreach (var key in m_accessoryScenes.Keys)
		{
			if (m_accessoryScenes[key].IsValid())
			{
				yield return UnloadScene(m_accessoryScenes[key].name);
				m_accessoryScenes[key] = default;
			}
		}

		yield return UnloadScene(m_optionScene.name);
		m_optionScene = default;
	}
	IEnumerator UnloadScene(string sceneName)
	{
		AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

		while (!operation.isDone) yield return null;

		yield break;
	}
}
