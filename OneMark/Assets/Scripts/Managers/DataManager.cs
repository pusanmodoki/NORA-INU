using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-110)]
public class DataManager : MonoBehaviour
{
	enum DevelopMode
	{
		NotDevelop,
		DevelopAwakeAll,
		DevelopAwakeEmpty
	}
	public enum Directory
	{
		StreamingAssets,
		PersistentData,
	}

	[System.Serializable]
	public struct FullFilePath
	{
		public FullFilePath(Directory directory, string filePath, string fileName)
		{
			m_directory = directory;
			m_filePath = filePath;
			m_fileName = fileName;
		}

		public Directory directory { get { return m_directory; } }
		public string filePath { get { return m_filePath; } }
		public string directoryAndFilePath { get { return SelectDirectory(m_directory) + "/" + filePath; } }
		public string fileName { get { return m_fileName; } }

		[SerializeField]
		Directory m_directory;
		[SerializeField]
		string m_filePath;
		[SerializeField]
		string m_fileName;

		static string SelectDirectory(Directory directory)
		{
			if (directory == Directory.StreamingAssets)
				return Application.streamingAssetsPath;
			else if (directory == Directory.PersistentData)
				return Application.persistentDataPath;
			else
				return "";
		}
	}

	public struct StageSettings
	{
		public StageSettings(Vector3 playerPosition, Quaternion playerRotation,
			string firstPointName)
		{
			this.playerPosition = playerPosition;
			this.playerRotation = playerRotation;
			this.firstPointName = firstPointName;
			this.playBgmName = "";
			this.isEnabled = true;
		}

		public Vector3 playerPosition { get; private set; }
		public Quaternion playerRotation { get; private set; }
		public string firstPointName { get; private set; }
		public string playBgmName { get; private set; }
		public bool isEnabled { get; private set; }
	}

	[System.Serializable]
	public class SaveData
	{
		public SaveData(int clearStageIndex)
		{
			this.numClearStages = clearStageIndex;
		}
		public SaveData(SaveData copy)
		{
			this.numClearStages = copy.numClearStages;
		}

		public static SaveData emptyData { get { return new SaveData(0); } }
		public static SaveData developData { get { return new SaveData(OneMarkSceneManager.cStageSceneIndexes.Count); } }

		public int numClearStages { get { return m_numClearStages; } private set { m_numClearStages = value; } }
		[SerializeField]
		int m_numClearStages = 0;

		public void UpdateNumClearStages()
		{
			var searchIndex = OneMarkSceneManager.instance.nowStageSceneIndex;

			for (int i = 0; i < OneMarkSceneManager.cStageSceneIndexes.Count; ++i)
			{
				if (OneMarkSceneManager.cStageSceneIndexes[i] == searchIndex)
				{
					if (numClearStages <= i)
						numClearStages = i;
					break;
				}
			}
		}

		public void CopyData(SaveData copy)
		{
			this.numClearStages = copy.numClearStages;
		}
	}

	/// <summary>Static instance</summary>
	public static DataManager instance { get; private set; } = null;
	public SaveData saveData {
		get { return m_saveData; }
		private set { if (m_saveData != null) m_saveData.CopyData(value); else m_saveData = value; } } 
	/// <summary>Static stage settings</summary>
	public ReadOnlyCollection<StageSettings> allStageSettings { get; private set; } = null;
	/// <summary>Static now stage settings</summary>
	public StageSettings nowStageSettings
	{
		get
		{
			string sceneName = OneMarkSceneManager.instance.nowLoadSceneName;
			int result0 = 0, result1 = 0;

			if (sceneName.Length >= 3 && sceneName[1] == '-'
				&& int.TryParse(sceneName.Substring(2, 1), out result0))
			{
				if (int.TryParse(sceneName.Substring(0, 1), out result1))
					result0 += result1 * 4;

				return allStageSettings[result0 - 1];
			}

			return default;
		}
	}


	[SerializeField]
	FullFilePath m_stagePresetFilePath = new FullFilePath(Directory.StreamingAssets, "Preset", "StageSettings");
	[SerializeField]
	FullFilePath m_saveDataFilePath = new FullFilePath(Directory.PersistentData, "app", "sd");

	static SaveData m_staticSaveData = null;
	SaveData m_saveData = null;

#if UNITY_EDITOR
	[Header("Debug Only"), SerializeField]
	DevelopMode m_dDevelopMode = DevelopMode.NotDevelop;
	[SerializeField]
	bool m_dIsResetEmptyDevelopMode = false;
	[SerializeField]
	bool m_dIsResetAllDevelopMode = false;
	[SerializeField]
	bool m_dIsWriteDevelopSaveData = false;
	[SerializeField]
	bool m_dIsResetEmptySaveData = false;
	[SerializeField]
	SaveData m_dDrawingSaveData = null;

	static DevelopMode m_dStaticDevelopMode = DevelopMode.NotDevelop;
	DevelopMode m_dSetDevelopMode = DevelopMode.NotDevelop;
	bool m_isEndAwake = false;
#endif


	public void WriteSaveData()
	{
#if UNITY_EDITOR
		if (m_dDevelopMode != DevelopMode.NotDevelop) return;
#endif

		DatFileEditor.SaveObject<SaveData>(m_saveDataFilePath.directoryAndFilePath,
			m_saveDataFilePath.fileName, ref m_saveData);
	}
	public static void ReadHyphenateVector3(string str, ref Vector3 result)
	{
		// StringSplitOption
		System.StringSplitOptions option = System.StringSplitOptions.None;
		// 数字に分ける
		string[] lines = str.Split(new char[] { '-' }, option);

		if (lines.Length <= 2)
			throw new System.IndexOutOfRangeException("ReadHyphenateVector3: lines.Length <= 2");

		result.Set(float.Parse(lines[0]), float.Parse(lines[1]), float.Parse(lines[2]));
	}

	/// <summary>[Awake]</summary>
	void Awake()
	{
		instance = this;

		if (allStageSettings == null)
		{
			ReadStageSettings();

			if (m_staticSaveData == null)
			{
#if UNITY_EDITOR
				ReadSaveData(m_dDevelopMode);

				m_dSetDevelopMode = m_dDevelopMode;
				m_dStaticDevelopMode = m_dDevelopMode;
				m_dDrawingSaveData = m_saveData;
				m_isEndAwake = true;
#else
				ReadSaveData(false);
#endif

				m_staticSaveData = saveData;
			}
			else
			{
				saveData = m_staticSaveData;
#if UNITY_EDITOR
				m_dDrawingSaveData = m_staticSaveData;
				m_dDevelopMode = m_dStaticDevelopMode;
#endif
			}
		}
	}

	void ReadStageSettings()
	{
		List<StageSettings> convertData = new List<StageSettings>();
		List<List<string>> readData;
		Vector3 playerPosition = Vector3.zero, playerRotation = Vector3.zero;

		DatFileEditor.LoadString(m_stagePresetFilePath.directoryAndFilePath, m_stagePresetFilePath.fileName, out readData);

		for (int i = 0, count = readData.Count; i < count; ++i)
		{
			ReadHyphenateVector3(readData[i][0], ref playerPosition);
			ReadHyphenateVector3(readData[i][1], ref playerRotation);
			convertData.Add(new StageSettings(playerPosition, Quaternion.Euler(playerRotation), readData[i][2]));
		}

		allStageSettings = new ReadOnlyCollection<StageSettings>(convertData.AsReadOnly());
	}

	void ReadSaveData(DevelopMode developMode)
	{
		if (developMode == DevelopMode.DevelopAwakeAll)
			saveData = SaveData.developData;
		else if (developMode == DevelopMode.DevelopAwakeEmpty)
			saveData = SaveData.emptyData;
		else
		{
			if (!DatFileEditor.IsExistsFile(m_saveDataFilePath.directoryAndFilePath, m_saveDataFilePath.fileName))
			{
				var emptyData = SaveData.emptyData;
				DatFileEditor.SaveObject<SaveData>(m_saveDataFilePath.directoryAndFilePath,
					m_saveDataFilePath.fileName, ref emptyData);

				saveData = new SaveData(emptyData);
			}
			else
			{
				SaveData read = null;
				DatFileEditor.LoadObject<SaveData>(m_saveDataFilePath.directoryAndFilePath,
					m_saveDataFilePath.fileName, out read);

				saveData = new SaveData(read);
			}
		}
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (m_isEndAwake & (UnityEditor.EditorApplication.isPlaying | UnityEditor.EditorApplication.isPaused))
			m_dDevelopMode = m_dSetDevelopMode;

		if (m_dIsResetEmptyDevelopMode)
		{
			m_dIsResetEmptyDevelopMode = false;

			saveData = SaveData.emptyData;
		}

		if (m_dIsResetAllDevelopMode)
		{
			m_dIsResetAllDevelopMode = false;

			saveData = SaveData.developData;
		}

		if (m_dIsWriteDevelopSaveData)
		{
			m_dIsWriteDevelopSaveData = false;

			DatFileEditor.SaveObject<SaveData>(m_saveDataFilePath.directoryAndFilePath,
				m_saveDataFilePath.fileName, ref m_saveData);
		}

		if (m_dIsResetEmptySaveData)
		{
			m_dIsResetEmptySaveData = false;

			if (m_dDevelopMode == DevelopMode.NotDevelop)
			{
				var emptyData = SaveData.emptyData;
				DatFileEditor.SaveObject<SaveData>(m_saveDataFilePath.directoryAndFilePath,
					m_saveDataFilePath.fileName, ref emptyData);

				saveData = new SaveData(emptyData);
			}
		}
	}
#endif
}
