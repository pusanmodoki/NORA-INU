using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-110)]
public class DataManager : MonoBehaviour
{
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
		public string fileName { get { return m_fileName; } }

		[SerializeField]
		Directory m_directory;
		[SerializeField]
		string m_filePath;
		[SerializeField]
		string m_fileName;
	}

	public struct StageSetting
	{
		public StageSetting(Vector3 playerPosition, Quaternion playerRotation,
			string firstPointName)
		{
			this.playerPosition = playerPosition;
			this.playerRotation = playerRotation;
			this.firstPointName = firstPointName;
		}

		public Vector3 playerPosition { get; private set; }
		public Quaternion playerRotation { get; private set; }
		public string firstPointName { get; private set; }
	}

	/// <summary>Static instance</summary>
	public static DataManager instance { get; private set; } = null;
	/// <summary>Static stage settings</summary>
	public static ReadOnlyCollection<StageSetting> stageSettings { get; private set; } = null;

	[SerializeField]
	FullFilePath stagePresetFilePath = new FullFilePath(Directory.StreamingAssets, "Preset", "StageSettings");

	public static StageSetting GetNowStageSetting()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		int result = 0;

		if (sceneName[0] != 'T') result = int.Parse(sceneName.Substring(0, 1)) * 4;
		result += int.Parse(sceneName.Substring(2, 1));

		return stageSettings[result - 1];
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

	public static string SelectDirectory(Directory directory)
	{
		if (directory == Directory.StreamingAssets)
			return Application.streamingAssetsPath;
		else if (directory == Directory.PersistentData)
			return Application.persistentDataPath;
		else
			return "";
	}

	/// <summary>[Awake]</summary>
	void Awake()
	{
		instance = this;

		if (stageSettings == null)
			ReadStageSettings();
	}

	void ReadStageSettings()
	{
		List<StageSetting> convertData = new List<StageSetting>();
		List<List<string>> readData;
		Vector3 playerPosition = Vector3.zero, playerRotation = Vector3.zero;

		DatFileEditor.LoadString(SelectDirectory(stagePresetFilePath.directory) + "/" + stagePresetFilePath.filePath,
			stagePresetFilePath.fileName, out readData);

		for (int i = 0, count = readData.Count; i < count; ++i)
		{
			ReadHyphenateVector3(readData[i][0], ref playerPosition);
			ReadHyphenateVector3(readData[i][1], ref playerRotation);
			convertData.Add(new StageSetting(playerPosition, Quaternion.Euler(playerRotation), readData[i][2]));
		}

		stageSettings = new ReadOnlyCollection<StageSetting>(convertData.AsReadOnly());
	}
}
