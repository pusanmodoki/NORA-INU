using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RaycastHitを拡張するRaycastHitExtension
/// </summary>
public static class RaycastHitExtension
{
	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): RaycastHit
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this RaycastHit raycastHit, GameObject gameObject)
	{
		int instanceID = gameObject.GetInstanceID();

		//InstanceIDで確認をとる
		if (raycastHit.rigidbody != null
			&& raycastHit.rigidbody.gameObject.GetInstanceID() == instanceID)
			return true;
		else if (raycastHit.transform.gameObject.GetInstanceID() == instanceID)
			return true;

		return false;
	}

	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): RaycastHit
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this RaycastHit raycastHit, int instanceID)
	{
		//InstanceIDで確認をとる
		if (raycastHit.rigidbody != null
			&& raycastHit.rigidbody.gameObject.GetInstanceID() == instanceID)
			return true;
		else if (raycastHit.transform.gameObject.GetInstanceID() == instanceID)
			return true;

		return false;
	}

	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): RaycastHit array
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this RaycastHit[] raycastHits, GameObject gameObject)
	{
		int instanceID = gameObject.GetInstanceID();

		//InstanceIDで確認をとる
		for (int i = 0, length = raycastHits.Length; i < length; ++i)
		{
			if (raycastHits[i].rigidbody != null
				&& raycastHits[i].rigidbody.gameObject.GetInstanceID() == instanceID)
				return true;
			else if (raycastHits[i].transform.gameObject.GetInstanceID() == instanceID)
				return true;
		}

		return false;
	}

	/// <summary>
	/// [ContainsInstanceID]
	/// 配列の中のオブジェクトと引数1で確認を行う
	/// 引数(this): RaycastHit array
	/// 引数1: 含まれているか確認するもの
	/// </summary>
	public static bool ContainsInstanceID(this RaycastHit[] raycastHits, int instanceID)
	{
		//InstanceIDで確認をとる
		for (int i = 0, length = raycastHits.Length; i < length; ++i)
		{
			if (raycastHits[i].rigidbody != null
				&& raycastHits[i].rigidbody.gameObject.GetInstanceID() == instanceID)
				return true;
			else if (raycastHits[i].transform.gameObject.GetInstanceID() == instanceID)
				return true;
		}

		return false;
	}
}
