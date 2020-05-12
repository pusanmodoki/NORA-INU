using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// 参照URL:	http://answers.unity3d.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
///					https://gist.github.com/shizenkikyo/19df69eb491bdd1f50df
/// </summary>
public static class ComponentExtension
{
	/// <summary>
	/// [CopyComponent]
	/// コンポーネントをコピーする
	/// 引数1: this
	/// 引数2: Copy component
	/// </summary>
	public static T CopyComponent<T>(this Component component, T copy) where T : Component
	{
		System.Type type = component.GetType();

		if (type != copy.GetType())
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! Component-> CopyComponent, Type mis-match.");
#endif
			return null;
		}

		// Target
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

		// Propertyを取得
		PropertyInfo[] propertyInfos = type.GetProperties(flags);
		foreach (var propertyInfo in propertyInfos)
		{
			// プロパティに書き込むことができる場合はtrue,それ以外の場合はfalse
			// プロパティにsetアクセサーがない場合は書き込めない
			if (propertyInfo.CanWrite)
			{
				try
				{
					// SetValueの第三引数, GetValueの第二引数はプロパティの引数->ない場合はnull
					propertyInfo.SetValue(component, propertyInfo.GetValue(copy, null), null);
				}
				catch
				{
					// In case of NotImplementedException being thrown. 
					//For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}
		}

		// フィールドの属性を取得し、フィールドのメタデータにアクセスできるようにする
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos)
		{
			finfo.SetValue(component, finfo.GetValue(copy));
		}
		return component as T;
	}
	/// <summary>
	/// [CopyComponent]
	/// コンポーネントをコピーする
	/// 引数1: this
	/// 引数2: Copy component
	/// 引数3: this type
	/// </summary>
	public static Component CopyComponent<T>(this Component component, T copy, System.Type thisType) where T : Component
	{
		if (thisType != copy.GetType())
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! Component-> CopyComponent, Type mis-match.");
#endif
			return null;
		}

		// Target
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

		// Propertyを取得
		PropertyInfo[] propertyInfos = thisType.GetProperties(flags);
		foreach (var propertyInfo in propertyInfos)
		{
			// プロパティに書き込むことができる場合はtrue,それ以外の場合はfalse
			// プロパティにsetアクセサーがない場合は書き込めない
			if (propertyInfo.CanWrite)
			{
				try
				{
					// SetValueの第三引数, GetValueの第二引数はプロパティの引数->ない場合はnull
					propertyInfo.SetValue(component, propertyInfo.GetValue(copy, null), null);
				}
				catch
				{
					// In case of NotImplementedException being thrown. 
					//For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}
		}

		// フィールドの属性を取得し、フィールドのメタデータにアクセスできるようにする
		FieldInfo[] finfos = thisType.GetFields(flags);
		foreach (var finfo in finfos)
		{
			finfo.SetValue(component, finfo.GetValue(copy));
		}
		return component as T;
	}

	/// <summary>
	/// [CopyComponent]
	/// コピーしたコンポーネントを追加する
	/// 引数1: this
	/// 引数2: Add component source
	/// </summary>
	public static T AddComponent<T>(this GameObject gameObject, T source) where T : Component
	{
		//Add(Copy)
		return gameObject.AddComponent<T>().CopyComponent(source) as T;
	}

	/// <summary>
	/// [MoveComponent]
	/// コンポーネントを別オブジェクトから移動させる
	/// 引数1: this
	/// 引数2: Move component
	/// </summary>
	public static T MoveComponent<T>(this GameObject gameObject, T moveComponent) where T : Component
	{
		//Add(Copy)
		T copy = gameObject.AddComponent<T>(moveComponent);

		// 移動後は削除
		Object.Destroy(moveComponent);

		return copy as T;
	}
}
