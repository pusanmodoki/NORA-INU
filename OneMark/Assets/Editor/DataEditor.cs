using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Editor
{
	public class DataEditor : EditorWindow
	{
		[SerializeField]
		DataEditorObject m_data = null;
		[SerializeField]
		string test;

		Vector2 m_scrollPosition = Vector2.zero;
		string[] m_findAssetPath = null;
		string m_assetPath = null;
		string m_createAssetPath = null;


		//! MenuItem("メニュー名/項目名") のフォーマットで記載してね
		[MenuItem("Custom/DataEditor")]
		static void ShowWindow()
		{
			// ウィンドウを表示！
			var window = EditorWindow.GetWindow<DataEditor>();
			window.Show();
		}

		void OnGUI()
		{
			if (m_data == null) m_data = LoadScriptableData();

			EditorGUILayout.LabelField("DatFileEditor   ...Datファイルを操作します");
			EditorGUILayout.Space();

			{
				string filePath = m_data.filePath;

				EditorGUI.BeginChangeCheck();
				filePath = EditorGUILayout.TextField("File path", filePath);
				if (EditorGUI.EndChangeCheck()) // テキストが変更された場合は
				{
					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					m_data.filePath = filePath; // データへテキストを保存する
					EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
				}
			}
			{
				string fileName = m_data.fileName;

				EditorGUI.BeginChangeCheck();
				fileName = EditorGUILayout.TextField("File name", fileName);
				if (EditorGUI.EndChangeCheck()) // テキストが変更された場合は
				{
					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					m_data.fileName = fileName; // データへテキストを保存する
					EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
				}
			}

			{
				DataEditorObject.FileMode fileMode = m_data.fileMode;
				EditorGUI.BeginChangeCheck();
				fileMode = (DataEditorObject.FileMode)EditorGUILayout.EnumPopup("File mode", fileMode);

				if (EditorGUI.EndChangeCheck())
				{
					if (m_data.isLoaded)
					{
						try
						{
							SaveData();
						}
						catch (System.Exception exception)
						{
							EditorUtility.DisplayDialog("Error!", exception.Message, "Close");
						}
					}

					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					m_data.fileMode = fileMode;
					EditorUtility.SetDirty(m_data);

					try
					{
						LoadData();
					}
					catch (System.Exception exception)
					{
						EditorUtility.DisplayDialog("Error!", exception.Message, "Close");
					}
				}
			}

			{
				DataEditorObject.DirectoryMode directoryMode = m_data.directoryMode;
				EditorGUI.BeginChangeCheck();
				directoryMode = (DataEditorObject.DirectoryMode)EditorGUILayout.EnumPopup("Directory mode", directoryMode);

				if (EditorGUI.EndChangeCheck())
				{
					if (m_data.isLoaded)
					{
						try
						{
							SaveData();
						}
						catch (System.Exception exception)
						{
							EditorUtility.DisplayDialog("Error!", exception.Message, "Close");
						}
					}

					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					m_data.directoryMode = directoryMode;
					EditorUtility.SetDirty(m_data);

					try
					{
						LoadData();
					}
					catch (System.Exception exception)
					{
						EditorUtility.DisplayDialog("Error!", exception.Message, "Close");
					}
				}
			}

			string directory = m_data.directoryMode == DataEditorObject.DirectoryMode.StreamingAssets ?
				Application.streamingAssetsPath : Application.persistentDataPath;
			string fullFilePath = m_data.filePath.Length > 0 ? $"{directory}/{m_data.filePath}/{m_data.fileName}.{"dat"}"
				: $"{directory}/{m_data.fileName}.{"dat"}";

			EditorGUILayout.Space();
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.wordWrap = true;

				EditorGUILayout.LabelField("Full path: " + fullFilePath, style);
			}

			if (!m_data.isLoaded && !System.IO.File.Exists(fullFilePath))
			{
				if (GUILayout.Button("Create File"))
				{
					if (!System.IO.Directory.Exists(directory + "/" + m_data.filePath))
						System.IO.Directory.CreateDirectory(directory + "/" + m_data.filePath);

					m_data.isLoaded = true;
					m_data.fileDataDoubleList.Clear();
					m_data.fileDataList.Clear();
					m_data.fileDataString = "";
					SaveData();
					LoadData();
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Load"))
				{
					try
					{
						LoadData();
					}
					catch(System.Exception exception)
					{
						EditorUtility.DisplayDialog("Error!", exception.Message, "Close");
					}
				}

				if (GUILayout.Button("Save"))
				{
					try
					{
						SaveData();
					}
					catch (System.Exception exception)
					{
						EditorUtility.DisplayDialog("Error!", exception.Message, "Close");
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			ShowData();
		}



		DataEditorObject LoadScriptableData()
		{
			if (m_findAssetPath == null)
			{
				m_findAssetPath = new string[1] { "Assets/Editor/UserFolder" };
				m_assetPath = Application.dataPath + "/Editor/UserFolder";
				m_createAssetPath = "Assets/Editor/UserFolder/DataEditorObject.asset";
			}

			// プロジェクトに存在する全ScriptableObjectのGUIDを取得
			DataEditorObject result = (DataEditorObject)AssetDatabase.FindAssets("t:ScriptableObject", m_findAssetPath)
			   // GUIDをパスに変換
			   .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			   // パスからPermanentDataの取得を試みる
			   .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(DataEditorObject)))
			   // null要素は取り除く
			   .Where(obj => obj != null)
			   // 取得したPermanentDataのうち、最初の一つだけを取る
			   .FirstOrDefault();

			if (result != null)
				return result;

			if (!System.IO.Directory.Exists(m_assetPath))
				System.IO.Directory.CreateDirectory(m_assetPath);

			result = ScriptableObject.CreateInstance<DataEditorObject>();
			AssetDatabase.CreateAsset(result, m_createAssetPath);
			return result;
		}

		void LoadData()
		{
			string filePath = m_data.directoryMode == DataEditorObject.DirectoryMode.StreamingAssets ?
				Application.streamingAssetsPath : Application.persistentDataPath;
			if (m_data.filePath.Length > 0)
				filePath += "/" + m_data.filePath;

			Undo.RecordObject(m_data, "Edit DataEditorWindow");
			m_data.numberOfSubContents.Clear();
			m_data.foldouts.Clear();

			try
			{
				switch (m_data.fileMode)
				{
					case DataEditorObject.FileMode.DoubleList:
						{
							m_data.fileDataList.Clear();
							m_data.fileDataString = null;

							List<SerializePackageString> result;
							DatFileEditor.LoadString(filePath, m_data.fileName, out result);
							m_data.fileDataDoubleList = result;
							m_data.numberOfContents = EnumerateContents();
							EnumerateSubContents();

							for (int i = 0, count = m_data.fileDataDoubleList.Count; i < count ; ++i)
								m_data.foldouts.Add(false);
							break;
						}
					case DataEditorObject.FileMode.List:
						{
							m_data.fileDataDoubleList.Clear();
							m_data.fileDataString = null;

							List<string> result;
							DatFileEditor.LoadString(filePath, m_data.fileName, out result);
							m_data.fileDataList = result;
							m_data.numberOfContents = EnumerateContents();
							break;
						}
					case DataEditorObject.FileMode.String:
						{
							m_data.fileDataDoubleList.Clear();
							m_data.fileDataList.Clear();

							string result;
							DatFileEditor.LoadString(filePath, m_data.fileName, out result);
							m_data.fileDataString = result;
							m_data.numberOfContents = EnumerateContents();
							break;
						}
					default:
						throw new System.InvalidOperationException("Invalid file mode.");
				}
			}
			catch (System.Exception)
			{
				m_data.isLoaded = false;
				m_data.numberOfContents = 0;
				m_scrollPosition = Vector2.zero;
				GUI.FocusControl("");
				EditorUtility.SetDirty(m_data);
				throw;
			}

			m_data.isLoaded = true;
			m_scrollPosition = Vector2.zero;
			GUI.FocusControl("");
			EditorUtility.SetDirty(m_data);
		}

		void SaveData()
		{
			if (!m_data.isLoaded)
				throw new System.SystemException("File not loaded");

			string filePath = m_data.directoryMode == DataEditorObject.DirectoryMode.StreamingAssets ?
				Application.streamingAssetsPath : Application.persistentDataPath;
			if (m_data.filePath.Length > 0)
				filePath += "/" + m_data.filePath;


			Undo.RecordObject(m_data, "Edit DataEditorWindow");
			try
			{
				switch (m_data.fileMode)
				{
					case DataEditorObject.FileMode.DoubleList:
						{
							m_data.fileDataList.Clear();
							m_data.fileDataString = null;

							DatFileEditor.SaveString(filePath, m_data.fileName, m_data.fileDataDoubleList);
							break;
						}
					case DataEditorObject.FileMode.List:
						{
							m_data.fileDataDoubleList.Clear();
							m_data.fileDataString = null;

							DatFileEditor.SaveString(filePath, m_data.fileName, m_data.fileDataList);
							break;
						}
					case DataEditorObject.FileMode.String:
						{
							m_data.fileDataDoubleList.Clear();
							m_data.fileDataList.Clear();

							DatFileEditor.SaveString(filePath, m_data.fileName, m_data.fileDataString);
							break;
						}
					default:
						throw new System.InvalidOperationException("Invalid file mode.");
				}
			}
			catch (System.Exception)
			{
				GUI.FocusControl("");
				EditorUtility.SetDirty(m_data);
				throw;
			}
			GUI.FocusControl("");
			EditorUtility.SetDirty(m_data);
		}

		void ShowData()
		{
			if (!m_data.isLoaded)
			{
				EditorGUILayout.HelpBox("Not loaded.", MessageType.Info);
				return;
			}
			
			switch (m_data.fileMode)
			{
				case DataEditorObject.FileMode.DoubleList:
					{
						var size0 = EditorStyles.label.CalcSize(new GUIContent("100 "));
						var size1 = EditorStyles.label.CalcSize(new GUIContent("XXX"));
						var size2 = EditorStyles.label.CalcSize(new GUIContent("XDelete LineX"));
						bool isIncrement = false;

						EditorGUILayout.LabelField("Number of lines: " + m_data.numberOfContents);
						m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

						for (int i = 0; i < m_data.fileDataDoubleList.Count; i = isIncrement ? i + 1 : i)
						{
							EditorGUILayout.BeginHorizontal();
							m_data.foldouts[i] = EditorGUILayout.Foldout(m_data.foldouts[i], "Line " + i);

							GUI.backgroundColor = Color.red;
							if (GUILayout.Button("Delete Line", GUILayout.Width(size2.x)))
							{
								Undo.RecordObject(m_data, "Edit DataEditorWindow");
								m_data.fileDataDoubleList.RemoveAt(i);
								m_data.foldouts.RemoveAt(i);
								--m_data.numberOfContents;
								m_data.numberOfSubContents.RemoveAt(i);
								EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
								isIncrement = false;
								continue;
							}
							else
								isIncrement = true;

							GUI.backgroundColor = Color.white;
							EditorGUILayout.EndHorizontal();

							if (m_data.foldouts[i])
							{
								++EditorGUI.indentLevel;
								EditorGUILayout.LabelField("Number of contents: " + m_data.numberOfSubContents[i]);
								ShowList(m_data.fileDataDoubleList[i].list, ref size0, ref size1, m_data.numberOfSubContents, i);
								--EditorGUI.indentLevel;
							}
						}

						if (GUILayout.Button("Add line"))
						{
							Undo.RecordObject(m_data, "Edit DataEditorWindow");
							m_data.fileDataDoubleList.Add(new SerializePackageString());
							m_data.foldouts.Add(true);
							++m_data.numberOfContents;
							m_data.numberOfSubContents.Add(0);
							EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
						}

						EditorGUILayout.EndScrollView();
						break;
					}
				case DataEditorObject.FileMode.List:
					{
						var size0 = EditorStyles.label.CalcSize(new GUIContent("100 "));
						var size1 = EditorStyles.label.CalcSize(new GUIContent("XXX"));
						EditorGUILayout.LabelField("Number of contents: " + m_data.numberOfContents);

						m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

						ShowList(m_data.fileDataList, ref size0, ref size1, ref m_data.GetRawNumberOfContents());
						m_data.numberOfContents = EnumerateContents();

						EditorGUILayout.EndScrollView();
						break;
					}
				case DataEditorObject.FileMode.String:
					{
						string data = m_data.fileDataString;
						EditorGUILayout.LabelField("Number of characters: " + m_data.numberOfContents);

						EditorGUI.BeginChangeCheck();
						m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
						data = EditorGUILayout.TextArea(data);
						EditorGUILayout.EndScrollView();
						if (EditorGUI.EndChangeCheck()) // テキストが変更された場合は
						{
							Undo.RecordObject(m_data, "Edit DataEditorWindow");
							m_data.fileDataString = data; // データへテキストを保存する
							m_data.numberOfContents = EnumerateContents();
							EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
						}
						break;
					}
				default:
					EditorGUILayout.HelpBox("Invalid file mode.", MessageType.Error);
					return;
			}
		}

		void ShowList(List<string> fileData, ref Vector2 size0, ref Vector2 size1, ref int contents)
		{
			for (int i = 0; i < fileData.Count;)
			{
				string data = fileData[i];
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(i + ": ", GUILayout.Width(size0.x));

				EditorGUI.BeginChangeCheck();
				data = EditorGUILayout.TextField(data);
				if (EditorGUI.EndChangeCheck()) // テキストが変更された場合は
				{
					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					fileData[i] = data;
					EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
				}

				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("X", GUILayout.Width(size1.x)))
				{
					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					fileData.RemoveAt(i);
					--contents;
					EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
				}
				else
					++i;
				GUI.backgroundColor = Color.white;

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Add element"))
			{
				Undo.RecordObject(m_data, "Edit DataEditorWindow");
				fileData.Add("");	
				EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
			}
			EditorGUILayout.EndHorizontal();
		}
		void ShowList(List<string> fileData, ref Vector2 size0, ref Vector2 size1, List<int> contents, int contentsID)
		{
			for (int i = 0; i < fileData.Count;)
			{
				string data = fileData[i];
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(i + ": ", GUILayout.Width(size0.x));

				EditorGUI.BeginChangeCheck();
				data = EditorGUILayout.TextField(data);
				if (EditorGUI.EndChangeCheck()) // テキストが変更された場合は
				{
					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					fileData[i] = data;
					EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
				}

				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("X", GUILayout.Width(size1.x)))
				{
					Undo.RecordObject(m_data, "Edit DataEditorWindow");
					fileData.RemoveAt(i);
					--contents[contentsID];
					EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
				}
				else
					++i;
				GUI.backgroundColor = Color.white;

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Add element"))
			{
				Undo.RecordObject(m_data, "Edit DataEditorWindow");
				fileData.Add("");
				EditorUtility.SetDirty(m_data); // データの変更をUnityに教える
			}
			EditorGUILayout.EndHorizontal();
		}

		int EnumerateContents()
		{
			switch (m_data.fileMode)
			{
				case DataEditorObject.FileMode.DoubleList:
						return m_data.fileDataDoubleList.Count;
				case DataEditorObject.FileMode.List:
						return m_data.fileDataList.Count;
				case DataEditorObject.FileMode.String:
						return m_data.fileDataString.Length;
				default:
					return 0;
			}
		}
		void EnumerateSubContents()
		{
			for (int i = 0, count = m_data.fileDataDoubleList.Count; i < count; ++i)
				m_data.numberOfSubContents.Add(m_data.fileDataDoubleList[i].list.Count);
		}
	}
}