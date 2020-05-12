using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Colliderを拡張するColliderExtension
/// </summary>
public static class ColliderExtension
{
	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): Collider
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this Collider collider, GameObject gameObject)
	{
		int instanceID = collider.gameObject.GetInstanceID();
		
		//InstanceIDで確認をとる
		if (collider.attachedRigidbody != null
			&& collider.attachedRigidbody.gameObject.GetInstanceID() == instanceID)
			return true;
		else if (collider.transform.gameObject.GetInstanceID() == instanceID)
			return true;

		return false;
	}

	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): Collider
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this Collider collider, int instanceID)
	{
		//InstanceIDで確認をとる
		if (collider.attachedRigidbody != null
			&& collider.attachedRigidbody.gameObject.GetInstanceID() == instanceID)
			return true;
		else if (collider.transform.gameObject.GetInstanceID() == instanceID)
			return true;

		return false;
	}

	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): Collider array
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this Collider[] colliders, GameObject gameObject)
	{
		int instanceID = gameObject.GetInstanceID();

		//InstanceIDで確認をとる
		for (int i = 0, length = colliders.Length; i < length; ++i)
		{
			if (colliders[i].attachedRigidbody != null
				&& colliders[i].attachedRigidbody.gameObject.GetInstanceID() == instanceID)
				return true;
			else if (colliders[i].transform.gameObject.GetInstanceID() == instanceID)
				return true;
		}

		return false;
	}

	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): Collider array
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this Collider[] colliders, int instanceID)
	{
		//InstanceIDで確認をとる
		for (int i = 0, length = colliders.Length; i < length; ++i)
		{
			if (colliders[i].attachedRigidbody != null
				&& colliders[i].attachedRigidbody.gameObject.GetInstanceID() == instanceID)
				return true;
			else if (colliders[i].transform.gameObject.GetInstanceID() == instanceID)
				return true;
		}

		return false;
	}
}
