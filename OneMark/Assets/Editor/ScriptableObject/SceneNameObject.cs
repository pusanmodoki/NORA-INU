using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SceneNameObject : ScriptableObject
{
	public List<string> sceneNames { get { return m_sceneNames; } set { m_sceneNames = value; } }
	public string[] sceneNamesToArray { get { return m_sceneNamesToArray; } set { m_sceneNamesToArray = value; } }
	public bool isFoldoutArray { get { return m_isFoldoutArray; } set { m_isFoldoutArray = value; } }

	[SerializeField]
	List<string> m_sceneNames = null;
	[SerializeField]
	string[] m_sceneNamesToArray = null;
	[SerializeField]
	bool m_isFoldoutArray = false;
}
