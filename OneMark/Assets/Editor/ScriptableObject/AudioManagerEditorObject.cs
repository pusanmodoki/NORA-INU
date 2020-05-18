using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AudioManagerEditorObject : ScriptableObject
{
	public List<bool> isFoldoutAllBgmInfos { get { return m_isFoldoutAllBgmInfos; } }
	public bool isFoldoutAllBgms { get { return m_isFoldoutAllBgms; } set { m_isFoldoutAllBgms = value; } }

	public List<bool> isFoldoutBgmForEachSceneInfos { get { return m_isFoldoutBgmForEachSceneInfos; } }
	public bool isFoldoutBgmForEachScenes { get { return m_isFoldoutBgmForEachScenes; } set { m_isFoldoutBgmForEachScenes = value; } }

	public List<string> uniqueKeys { get { return m_uniqueKeys; } set { m_uniqueKeys = value; } }
	public string[] uniqueKeysToArray { get { return m_uniqueKeysToArray; } set { m_uniqueKeysToArray = value; } }
	public string[] sceneNames { get { return m_sceneNames; } set { m_sceneNames = value; } }
	public string[] scenePaths { get { return m_scenePaths; } set { m_scenePaths = value; } }

	[SerializeField]
	List<bool> m_isFoldoutAllBgmInfos = new List<bool>();
	[SerializeField]
	bool m_isFoldoutAllBgms = false;

	[SerializeField]
	List<bool> m_isFoldoutBgmForEachSceneInfos = new List<bool>();
	[SerializeField]
	bool m_isFoldoutBgmForEachScenes = false;

	[SerializeField]
	List<string> m_uniqueKeys = new List<string>();
	[SerializeField]
	string[] m_uniqueKeysToArray = null;
	[SerializeField]
	string[] m_sceneNames = null;
	[SerializeField]
	string[] m_scenePaths = null;
}
