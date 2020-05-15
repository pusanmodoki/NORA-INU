using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Editor
{
	[CustomPropertyDrawer(typeof(SceneName))]
	public class SceneNameEditor : PropertyDrawer
	{
		SceneNameObject m_data = null;
		string[] m_findAssetPath = null;
		string m_assetPath = null;
		string m_createAssetPath = null;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (m_data == null) 
			{
				m_data = LoadScriptableData();
				GetSceneNames(false);
			}

			var sceneName = property.FindPropertyRelative("m_sceneName");
			var sizeReload = EditorStyles.label.CalcSize(new GUIContent("XReload scenesX"));

			Rect popUpRect = new Rect(
					EditorGUIUtility.labelWidth,
					position.y,
					position.width - EditorGUIUtility.labelWidth - sizeReload.x,
					EditorGUIUtility.singleLineHeight);

			Rect buttonRect = new Rect(
					position.x + position.width - sizeReload.x,
					position.y,
					sizeReload.x,
					EditorGUIUtility.singleLineHeight);


			int result = Mathf.Clamp(m_data.sceneNames.IndexOf(sceneName.stringValue)
				, 0, m_data.sceneNames.Count - 1);
			
			EditorGUI.LabelField(position, property.displayName);
			result = EditorGUI.Popup(popUpRect, result, m_data.sceneNamesToArray);
			sceneName.stringValue = m_data.sceneNamesToArray[result];

			GUI.color = Color.green;
			if (GUI.Button(buttonRect, "Reload scenes"))
			{
				GetSceneNames(true);
			}
			GUI.color = Color.white;
		}

		SceneNameObject LoadScriptableData()
		{
			if (m_findAssetPath == null)
			{
				m_findAssetPath = new string[1] { "Assets/Editor/UserFolder" };
				m_assetPath = Application.dataPath + "/Editor/UserFolder";
				m_createAssetPath = "Assets/Editor/UserFolder/SceneNameObject.asset";
			}
			// プロジェクトに存在する全ScriptableObjectのGUIDを取得
			SceneNameObject result = (SceneNameObject)AssetDatabase.FindAssets("t:ScriptableObject", m_findAssetPath)
			   // GUIDをパスに変換
			   .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			   // パスからPermanentDataの取得を試みる
			   .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SceneNameObject)))
			   // null要素は取り除く
			   .Where(obj => obj != null)
			   // 取得したPermanentDataのうち、最初の一つだけを取る
			   .FirstOrDefault();

			if (result != null)
				return result;

			if (!System.IO.Directory.Exists(m_assetPath))
				System.IO.Directory.CreateDirectory(m_assetPath);

			result = ScriptableObject.CreateInstance<SceneNameObject>();
			AssetDatabase.CreateAsset(result, m_createAssetPath);
			return result;
		}

		void GetSceneNames(bool isDrawCompletedLog)
		{
			if (m_data == null) return;

			var scenes = EditorBuildSettings.scenes;
			if (m_data.sceneNamesToArray == null || m_data.sceneNamesToArray.Length != scenes.Length)
			{
				m_data.sceneNamesToArray = new string[scenes.Length];
			}

			m_data.sceneNames.Clear();
			for (int i = 0, length = scenes.Length; i < length; ++i)
			{
				int slash = scenes[i].path.LastIndexOf('/');
				string name = scenes[i].path.Substring(slash + 1, scenes[i].path.Length - 6 - (slash + 1));
				m_data.sceneNamesToArray[i] = name;
				m_data.sceneNames.Add(name);
			}

			EditorUtility.SetDirty(m_data);

			if (isDrawCompletedLog)
				Debug.Log("AudioManager->reload scene infomations completed.");
		}
	}
}