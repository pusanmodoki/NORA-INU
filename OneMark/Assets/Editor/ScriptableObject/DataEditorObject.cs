using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataEditorObject : ScriptableObject
{
	public enum FileMode
	{
		DoubleList,
		List, 
		String
	}
	public enum DirectoryMode
	{
		StreamingAssets,
		PersistentData,
	}

	public string filePath { get { return m_filePath; } set { m_filePath = value; } }
	public string fileName { get { return m_fileName; } set { m_fileName = value; } }
	public DirectoryMode directoryMode { get { return m_directoryMode; } set { m_directoryMode = value; } }
	public FileMode fileMode { get { return m_fileMode; } set { m_fileMode = value; } }
	public List<SerializePackageString> fileDataDoubleList { get { return m_fileDataDoubleList; } set { m_fileDataDoubleList = value; } }
	public List<string> fileDataList { get { return m_fileDataList; } set { m_fileDataList = value; } }
	public string fileDataString { get { return m_fileDataString; } set { m_fileDataString = value; } }
	public List<bool> foldouts { get { return m_foldouts; } set { m_foldouts = value; } }
	public int numberOfContents { get { return m_numberOfContents; } set { m_numberOfContents = value; } }
	public List<int> numberOfSubContents { get { return m_numberOfSubContents; } set { m_numberOfSubContents = value; } }
	public bool isLoaded { get { return m_isLoaded; } set { m_isLoaded = value; } }

	public ref int GetRawNumberOfContents() { return ref m_numberOfContents; }

	[SerializeField]
	string m_filePath = "";
	[SerializeField]
	string m_fileName = "";
	[SerializeField]
	DirectoryMode m_directoryMode = DirectoryMode.StreamingAssets;
	[SerializeField]
	FileMode m_fileMode = FileMode.DoubleList;
	[SerializeField]
	List<SerializePackageString> m_fileDataDoubleList = new List<SerializePackageString>();
	[SerializeField]
	List<string> m_fileDataList = new List<string>();
	[SerializeField]
	string m_fileDataString = "";
	[SerializeField]
	List<bool> m_foldouts = new List<bool>();
	[SerializeField]
	int m_numberOfContents = 0;
	[SerializeField]
	List<int> m_numberOfSubContents = new List<int>();
	[SerializeField]
	bool m_isLoaded = false;

}
