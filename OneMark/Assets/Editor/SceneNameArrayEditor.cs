using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Editor
{
	[CustomPropertyDrawer(typeof(SceneNameArray))]
	public class SceneNameArrayEditor : PropertyDrawer
	{
		SceneNameObject m_data = null;
		string[] m_findAssetPath = null;
		string m_assetPath = null;
		string m_createAssetPath = null;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (m_data.name == null)
			{
				bool isAllNull = true;
				for (int i = 0; i < m_data.sceneNamesToArray.Length; ++i)
					if (m_data.sceneNamesToArray[i] != null) isAllNull = false;
				for (int i = 0; i < m_data.sceneNames.Count; ++i)
					if (m_data.sceneNames[i] != null) isAllNull = false;

				if (isAllNull) m_data = null;
			}

			if (m_data == null) 
			{
				m_data = LoadScriptableData();
				GetSceneNames(false);
			}

			var sceneNames = property.FindPropertyRelative("m_sceneNames");
			var sizeReload = EditorStyles.label.CalcSize(new GUIContent("XReload scenesX"));
			var sizeX = EditorStyles.label.CalcSize(new GUIContent("XXX"));
			var sizeIndent = EditorStyles.label.CalcSize(new GUIContent("x"));

			Rect foldoutRect = new Rect(
					position.x,
					position.y,
					position.width - sizeReload.x * 2,
					EditorGUIUtility.singleLineHeight);

			Rect buttonRect = new Rect(
					position.x + position.width - sizeReload.x,
					position.y,
					sizeReload.x,
					EditorGUIUtility.singleLineHeight);

			EditorGUI.BeginChangeCheck();
			m_data.isFoldoutArray = EditorGUI.Foldout(foldoutRect, m_data.isFoldoutArray, property.displayName);
			if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

			GUI.color = Color.green;
			if (GUI.Button(buttonRect, "Reload scenes"))
				GetSceneNames(true);

			buttonRect.x -= buttonRect.width;
			GUI.color = Color.cyan;
			if (GUI.Button(buttonRect, "Add element"))
			{
				sceneNames.InsertArrayElementAtIndex(sceneNames.arraySize);
				sceneNames.GetArrayElementAtIndex(sceneNames.arraySize - 1).stringValue = "";
			}
			GUI.color = Color.white;

			if (!m_data.isFoldoutArray) return;
			
			if (sceneNames.arraySize > 0)
			{
				Rect popUpRect = new Rect(
						position.x + sizeIndent.x,
						position.y + EditorGUIUtility.singleLineHeight,
						position.width - sizeIndent.x - sizeX.x,
						EditorGUIUtility.singleLineHeight);
				Rect deleteRect = new Rect(
						position.x + position.width - sizeX.x,
						position.y + EditorGUIUtility.singleLineHeight,
						sizeX.x,
						EditorGUIUtility.singleLineHeight);

				for (int i = 0; i < sceneNames.arraySize; ++i)
				{
					var element = sceneNames.GetArrayElementAtIndex(i);
					int result = Mathf.Clamp(m_data.sceneNames.IndexOf(element.stringValue)
						, 0, m_data.sceneNames.Count - 1);

					//EditorGUI.LabelField(position, "Scene name");
					result = EditorGUI.Popup(popUpRect, result, m_data.sceneNamesToArray);
					element.stringValue = m_data.sceneNamesToArray[result];

					GUI.color = Color.red;
					if (GUI.Button(deleteRect, "X"))
					{
						sceneNames.DeleteArrayElementAtIndex(i);
						GUI.color = Color.white;
						--i;
						continue;
					}
					GUI.color = Color.white;

					popUpRect.y += EditorGUIUtility.singleLineHeight;
					deleteRect.y += EditorGUIUtility.singleLineHeight;
				}
			}
			else
			{
				Rect helpRect = new Rect(
					position.x + sizeIndent.x,
					position.y + EditorGUIUtility.singleLineHeight,
					position.width - sizeIndent.x,
					EditorGUIUtility.singleLineHeight * 2.0f);

				EditorGUI.HelpBox(helpRect, "Empty.", MessageType.Info);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (m_data == null)
			{
				m_data = LoadScriptableData();
				GetSceneNames(false);
			}

			var sceneNames = property.FindPropertyRelative("m_sceneNames");
			float result = 0.0f;

			//Foldout
			result = EditorGUIUtility.singleLineHeight;

			//Names
			if (sceneNames.arraySize > 0 && m_data.isFoldoutArray)
				result += EditorGUIUtility.singleLineHeight * sceneNames.arraySize;

			//Help box
			if (sceneNames.arraySize == 0 && m_data.isFoldoutArray)
				result += EditorGUIUtility.singleLineHeight * 2.0f;

			return result;
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