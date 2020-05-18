using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneName : IComparer<SceneName>
{
	public string sceneName { get { return m_sceneName; } }

	[SerializeField]
	string m_sceneName = "";

	public int Compare(SceneName x, SceneName y)
	{
		return string.Compare(x.m_sceneName, y.m_sceneName);
	}
	public static implicit operator string(SceneName name)
	{
		return name.m_sceneName;
	}
}


[System.Serializable]
public class SceneNameArray
{
	public string[] sceneNames { get { return m_sceneNames; } }
	public int length { get { return m_sceneNames.Length; } }
	public string this[int index]
	{
		get { return m_sceneNames[index]; }
		set { m_sceneNames[index] = value; }
	}

	[SerializeField]
	string[] m_sceneNames = null;
}