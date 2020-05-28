using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Editor
{
	[CustomEditor(typeof(AudioManager))]
	public class AudioManagerEditor : UnityEditor.Editor
	{
		static readonly string m_cUniqueKey = "m_uniqueKey";
		static readonly string m_cClip = "m_clip";
		static readonly string m_cVolume = "m_volume";

		static readonly string m_cMinVolume = "m_minVolume";
		static readonly string m_cChangeSeconds = "m_changeSeconds";

		static readonly string m_cSceneName = "m_sceneName";
		static readonly string m_cLoadBgmKeys = "m_loadBgmKeys";

		static readonly string m_cDefaultKey = "Default key";

		SerializedProperty m_bgmSource = null;
		SerializedProperty m_allBgms = null;
		SerializedProperty m_allVolumeChangePresets = null;
		SerializedProperty m_bgmForEachScenes = null;

		AudioManagerEditorObject m_data = null;
		string[] m_findAssetPath = null;
		string m_assetPath = null;
		string m_createAssetPath = null;

		void OnEnable()
		{
			m_bgmSource = serializedObject.FindProperty("m_bgmSource");
			m_allBgms = serializedObject.FindProperty("m_allBgms");
			m_allVolumeChangePresets = serializedObject.FindProperty("m_allVolumeChangePresets");
			m_bgmForEachScenes = serializedObject.FindProperty("m_bgmForEachScenes");
		}
		public override void OnInspectorGUI()
		{
			var sizeX = EditorStyles.label.CalcSize(new GUIContent("XXX"));
			var sizeElementCount = EditorStyles.label.CalcSize(new GUIContent("element count: 000"));
			var sizeAddElement = EditorStyles.label.CalcSize(new GUIContent("XAdd elementX"));

			serializedObject.Update();

			if (m_data == null) m_data = LoadScriptableData();
			if ((m_data.sceneNames == null || m_data.sceneNames.Length == 0
				|| m_bgmForEachScenes.arraySize == 0) && !EditorApplication.isPlaying)
				CheckSceneNames(false);

			ShowVolumePresets(ref sizeX, ref sizeElementCount);

			ShowPlaySourceAndClips(ref sizeX, ref sizeElementCount);

			ShowBgmToLoadInEachScene(ref sizeX, ref sizeElementCount, ref sizeAddElement);

			serializedObject.ApplyModifiedProperties();
		}


		void ShowVolumePresets(ref Vector2 sizeX, ref Vector2 sizeElementCount)
		{
			var style = new GUIStyle(GUI.skin.label);
			style.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("Volume Change Presets", style);

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				m_data.isFoldoutAllVolumePreset = EditorGUILayout.Foldout(m_data.isFoldoutAllVolumePreset, "Presets");
				if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

				EditorGUILayout.LabelField("element count: "
					+ m_allVolumeChangePresets.arraySize.ToString().PadLeft(3, ' '), GUILayout.Width(sizeElementCount.x));
			}
			EditorGUILayout.EndHorizontal();

			if (m_data.isFoldoutAllVolumePreset)
			{
				if (m_allVolumeChangePresets.arraySize == 0)
					EditorGUILayout.HelpBox("Preset empty.", MessageType.Info);

				++EditorGUI.indentLevel;
				for (int i = 0; i < m_allVolumeChangePresets.arraySize; ++i)
				{
					var element = m_allVolumeChangePresets.GetArrayElementAtIndex(i);
					var uniqueKey = element.FindPropertyRelative(m_cUniqueKey);
					var minVolume = element.FindPropertyRelative(m_cMinVolume);
					var changeSeconds = element.FindPropertyRelative(m_cChangeSeconds);

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUI.BeginChangeCheck();
						while (m_data.isFoldoutAllVolumePresetInfos.Count <= i) m_data.isFoldoutAllVolumePresetInfos.Add(false);
						m_data.isFoldoutAllVolumePresetInfos[i] = EditorGUILayout.Foldout(m_data.isFoldoutAllVolumePresetInfos[i],
							"ID: " + (i).ToString().PadLeft(3, ' ') + ",  Unique key: " + uniqueKey.stringValue);
						if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

						GUI.backgroundColor = Color.red;
						if (GUILayout.Button("X", GUILayout.Width(sizeX.x)))
						{
							if (m_data.isFoldoutAllVolumePresetInfos.Count > i)
							{
								m_data.isFoldoutAllVolumePresetInfos.RemoveAt(i);
								EditorUtility.SetDirty(m_data);
							}

							m_allVolumeChangePresets.DeleteArrayElementAtIndex(i);
							GUI.backgroundColor = Color.white;
							--i;
							continue;
						}
						GUI.backgroundColor = Color.white;
					}
					EditorGUILayout.EndHorizontal();


					if (m_data.isFoldoutAllVolumePresetInfos[i])
					{
						EditorGUILayout.PropertyField(uniqueKey);
						EditorGUILayout.PropertyField(minVolume);
						EditorGUILayout.PropertyField(changeSeconds);
					}
				}
				--EditorGUI.indentLevel;

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if (GUILayout.Button("Add element"))
				{
					m_allVolumeChangePresets.InsertArrayElementAtIndex(m_allVolumeChangePresets.arraySize);
					m_data.isFoldoutAllVolumePresetInfos.Add(true);
					EditorUtility.SetDirty(m_data);

					var addData = m_allVolumeChangePresets.GetArrayElementAtIndex(m_allVolumeChangePresets.arraySize - 1);
					addData.FindPropertyRelative(m_cMinVolume).floatValue = 0.0f;
					addData.FindPropertyRelative(m_cChangeSeconds).floatValue = 1.0f;

					int count = 0;
					for (int i = 0; i < m_allVolumeChangePresets.arraySize - 1; ++i)
						if (m_allVolumeChangePresets.GetArrayElementAtIndex(i).FindPropertyRelative(m_cUniqueKey).stringValue.StartsWith(m_cDefaultKey))
							++count;

					if (count == 0)
						addData.FindPropertyRelative(m_cUniqueKey).stringValue = m_cDefaultKey;
					else
						addData.FindPropertyRelative(m_cUniqueKey).stringValue = m_cDefaultKey + '(' + count + ')';
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		void ShowPlaySourceAndClips(ref Vector2 sizeX, ref Vector2 sizeElementCount)
		{
			m_data.uniqueKeys.Clear();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			var style = new GUIStyle(GUI.skin.label);
			style.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("Play Source And Clips", style);

			EditorGUILayout.PropertyField(m_bgmSource);

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				m_data.isFoldoutAllBgms = EditorGUILayout.Foldout(m_data.isFoldoutAllBgms, "All Bgms");
				if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

				EditorGUILayout.LabelField("element count: "
					+ m_allBgms.arraySize.ToString().PadLeft(3, ' '), GUILayout.Width(sizeElementCount.x));
			}
			EditorGUILayout.EndHorizontal();

			if (m_data.isFoldoutAllBgms)
			{
				if (m_allBgms.arraySize == 0)
					EditorGUILayout.HelpBox("Bgm empty.", MessageType.Info);

				++EditorGUI.indentLevel;
				for (int i = 0; i < m_allBgms.arraySize; ++i)
				{
					var element = m_allBgms.GetArrayElementAtIndex(i);
					var uniqueKey = element.FindPropertyRelative(m_cUniqueKey);
					var clip = element.FindPropertyRelative(m_cClip);
					var volume = element.FindPropertyRelative(m_cVolume);

					if (!m_data.uniqueKeys.Contains(uniqueKey.stringValue))
						m_data.uniqueKeys.Add(uniqueKey.stringValue);

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUI.BeginChangeCheck();
						while (m_data.isFoldoutAllBgmInfos.Count <= i) m_data.isFoldoutAllBgmInfos.Add(false);
						m_data.isFoldoutAllBgmInfos[i] = EditorGUILayout.Foldout(m_data.isFoldoutAllBgmInfos[i],
							"ID: " + (i).ToString().PadLeft(3, ' ') + ",  Unique key: " + uniqueKey.stringValue);
						if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

						GUI.backgroundColor = Color.red;
						if (GUILayout.Button("X", GUILayout.Width(sizeX.x)))
						{
							if (m_data.isFoldoutAllBgmInfos.Count > i)
							{
								m_data.isFoldoutAllBgmInfos.RemoveAt(i);
								EditorUtility.SetDirty(m_data);
							}

							m_allBgms.DeleteArrayElementAtIndex(i);
							GUI.backgroundColor = Color.white;
							--i;
							continue;
						}
						GUI.backgroundColor = Color.white;
					}
					EditorGUILayout.EndHorizontal();


					if (m_data.isFoldoutAllBgmInfos[i])
					{
						//++EditorGUI.indentLevel;
						EditorGUILayout.PropertyField(uniqueKey);

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(clip);
						if (EditorGUI.EndChangeCheck() && clip.objectReferenceValue != null)
						{
							AudioImporter importer = AudioImporter.GetAtPath(
								AssetDatabase.GetAssetPath(clip.objectReferenceValue as AudioClip)) as AudioImporter;

							var settings = importer.defaultSampleSettings;
							settings.loadType = AudioClipLoadType.Streaming;
							settings.compressionFormat = AudioCompressionFormat.Vorbis;
							settings.quality = 100;
							importer.defaultSampleSettings = settings;

							importer.preloadAudioData = false;
						}

						EditorGUILayout.PropertyField(volume);
						//--EditorGUI.indentLevel;
					}
				}
				--EditorGUI.indentLevel;

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if (GUILayout.Button("Add element"))
				{
					m_allBgms.InsertArrayElementAtIndex(m_allBgms.arraySize);
					m_data.isFoldoutAllBgmInfos.Add(true);
					EditorUtility.SetDirty(m_data);

					var addData = m_allBgms.GetArrayElementAtIndex(m_allBgms.arraySize - 1);
					addData.FindPropertyRelative(m_cClip).objectReferenceValue = null;
					addData.FindPropertyRelative(m_cVolume).floatValue = 1.0f;

					int count = 0;
					for (int i = 0; i < m_allBgms.arraySize - 1; ++i)
						if (m_allBgms.GetArrayElementAtIndex(i).FindPropertyRelative(m_cUniqueKey).stringValue.StartsWith(m_cDefaultKey))
							++count;

					if (count == 0)
						addData.FindPropertyRelative(m_cUniqueKey).stringValue = m_cDefaultKey;
					else
						addData.FindPropertyRelative(m_cUniqueKey).stringValue = m_cDefaultKey + '(' + count + ')';
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				for (int i = 0; i < m_allBgms.arraySize; ++i)
				{
					var element = m_allBgms.GetArrayElementAtIndex(i);
					var uniqueKey = element.FindPropertyRelative(m_cUniqueKey);

					if (!m_data.uniqueKeys.Contains(uniqueKey.stringValue))
						m_data.uniqueKeys.Add(uniqueKey.stringValue);
				}
			}
		}



		void ShowBgmToLoadInEachScene(ref Vector2 sizeX, ref Vector2 sizeElementCount, ref Vector2 sizeAddElement)
		{
			m_data.uniqueKeysToArray = m_data.uniqueKeys.ToArray();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			{
				var style = new GUIStyle(GUI.skin.label);
				style.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("Bgm To Load In Each Scene", style);

				if (GUILayout.Button("reload scene infomations"))
				{
					CheckSceneNames(true);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				m_data.isFoldoutBgmForEachScenes = EditorGUILayout.Foldout(
					m_data.isFoldoutBgmForEachScenes, "Bgm For Each Scene");
				if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

				EditorGUILayout.LabelField("element count: "
					+ m_bgmForEachScenes.arraySize.ToString().PadLeft(3, ' '), GUILayout.Width(sizeElementCount.x));
			}
			EditorGUILayout.EndHorizontal();

			if (m_bgmForEachScenes.arraySize == 0)
			{
				EditorGUILayout.HelpBox("Push reload scene infomations.", MessageType.Info);
				return;
			}

			if (m_data.isFoldoutBgmForEachScenes)
			{
				++EditorGUI.indentLevel;

				for (int i = 0; i < m_bgmForEachScenes.arraySize; ++i)
				{
					var element = m_bgmForEachScenes.GetArrayElementAtIndex(i);
					var keys = element.FindPropertyRelative(m_cLoadBgmKeys);

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUI.BeginChangeCheck();
						while (m_data.isFoldoutBgmForEachSceneInfos.Count <= i) m_data.isFoldoutBgmForEachSceneInfos.Add(false);
						m_data.isFoldoutBgmForEachSceneInfos[i] = EditorGUILayout.Foldout(
							m_data.isFoldoutBgmForEachSceneInfos[i], element.FindPropertyRelative(m_cSceneName).stringValue);
						if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(m_data);

						if (GUILayout.Button("Add element", GUILayout.Width(sizeAddElement.x)))
						{
							if (m_data.uniqueKeys.Count > 0)
							{
								keys.InsertArrayElementAtIndex(keys.arraySize);

								var addData = keys.GetArrayElementAtIndex(keys.arraySize - 1);
								addData.stringValue = m_cDefaultKey;
							}
							else
								Debug.LogWarning("AudioManager->Add element failed!! Bgmが選択されていません");
						}
					}
					EditorGUILayout.EndHorizontal();

					if (m_data.isFoldoutBgmForEachSceneInfos[i])
					{
						if (keys.arraySize == 0)
							EditorGUILayout.HelpBox("Unique key empty.", MessageType.Info);

						for (int k = 0; k < keys.arraySize; ++k)
						{
							EditorGUILayout.BeginHorizontal();
							{
								int result = Mathf.Clamp(m_data.uniqueKeys.IndexOf(keys.GetArrayElementAtIndex(k).stringValue)
									, 0, m_data.uniqueKeys.Count - 1);

								if (k > 0) result = EditorGUILayout.Popup("element" + k.ToString().PadLeft(3, ' ') + ":", result, m_data.uniqueKeysToArray);
								else result = EditorGUILayout.Popup("element" + k.ToString().PadLeft(3, ' ') + "(auto play bgm):", result, m_data.uniqueKeysToArray);
								keys.GetArrayElementAtIndex(k).stringValue = m_data.uniqueKeys[result];

								GUI.backgroundColor = Color.red;
								if (GUILayout.Button("X", GUILayout.Width(sizeX.x)))
								{
									keys.DeleteArrayElementAtIndex(k);
									--k;
								}
								GUI.backgroundColor = Color.white;
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}

				--EditorGUI.indentLevel;
			}
		}




		void CheckSceneNames(bool isDrawCompletedLog)
		{
			if (EditorApplication.isPlaying)
			{
				Debug.LogWarning("AudioManager->reload scene infomations failed!! 実行中はできません");
				return;
			}

			var scenes = EditorBuildSettings.scenes;
			List<string> oldSceneNames = m_data.sceneNames != null ? m_data.sceneNames.ToList() : new List<string>();
			bool isAdd = false;

			if (m_data.scenePaths == null || m_data.scenePaths.Length != scenes.Length)
			{
				m_data.sceneNames = new string[scenes.Length];
				m_data.scenePaths = new string[scenes.Length];
			}

			for (int i = 0, length = scenes.Length; i < length; ++i)
			{
				int slash = scenes[i].path.LastIndexOf('/');
				string name = scenes[i].path.Substring(slash + 1, scenes[i].path.Length - 6 - (slash + 1));
				m_data.sceneNames[i] = name;

				if (oldSceneNames.Contains(name))
					oldSceneNames.Remove(name);

				isAdd = true;
				for (int k = 0; k < m_bgmForEachScenes.arraySize; ++k)
				{
					if (name == m_bgmForEachScenes.GetArrayElementAtIndex(k)
						.FindPropertyRelative(m_cSceneName).stringValue)
					{
						isAdd = false;
						break;
					}
				}

				if (isAdd)
				{
					m_bgmForEachScenes.InsertArrayElementAtIndex(m_bgmForEachScenes.arraySize);
					m_bgmForEachScenes.GetArrayElementAtIndex(m_bgmForEachScenes.arraySize - 1)
						.FindPropertyRelative(m_cSceneName).stringValue = name;
					m_data.isFoldoutBgmForEachSceneInfos.Add(false);
				}

				m_data.scenePaths[i] = scenes[i].path.Replace("Assets/", "").Replace("/", "_");
			}

			for (int i = 0, count = oldSceneNames.Count; i < count; ++i)
				for (int k = 0; k < m_bgmForEachScenes.arraySize; ++k)
				{
					if (oldSceneNames[i] == m_bgmForEachScenes.GetArrayElementAtIndex(k)
						.FindPropertyRelative(m_cSceneName).stringValue)
					{
						m_bgmForEachScenes.DeleteArrayElementAtIndex(k);
						m_data.isFoldoutBgmForEachSceneInfos.RemoveAt(k);
						--k;
					}
				}

			EditorUtility.SetDirty(m_data);

			if (isDrawCompletedLog)
				Debug.Log("AudioManager->reload scene infomations completed.");
		}




		AudioManagerEditorObject LoadScriptableData()
		{
			if (m_findAssetPath == null)
			{
				m_findAssetPath = new string[1] { "Assets/Editor/UserFolder" };
				m_assetPath = Application.dataPath + "/Editor/UserFolder";
				m_createAssetPath = "Assets/Editor/UserFolder/AudioManagerEditorObject.asset";
			}
			// プロジェクトに存在する全ScriptableObjectのGUIDを取得
			AudioManagerEditorObject result = (AudioManagerEditorObject)AssetDatabase.FindAssets("t:ScriptableObject", m_findAssetPath)
			   // GUIDをパスに変換
			   .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			   // パスからPermanentDataの取得を試みる
			   .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(AudioManagerEditorObject)))
			   // null要素は取り除く
			   .Where(obj => obj != null)
			   // 取得したPermanentDataのうち、最初の一つだけを取る
			   .FirstOrDefault(); 

			if (result != null)
				return result;

			if (!System.IO.Directory.Exists(m_assetPath))
				System.IO.Directory.CreateDirectory(m_assetPath);

			result = ScriptableObject.CreateInstance<AudioManagerEditorObject>();
			AssetDatabase.CreateAsset(result, m_createAssetPath);
			return result;
		}
	}
}