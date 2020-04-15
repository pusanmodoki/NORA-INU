using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [BeginSetBillboard]
/// StartでのみセットするBeginSetBillboard
/// </summary>
public class BeginSetBillboard : MonoBehaviour
{
	/// <summary>true->RectTransformから操作, false->Transformから操作</summary>
	[SerializeField]
	bool m_isUseRectTransform = true;

	/// <summary>[Start]</summary>
	void Start()
	{
		Vector3 position = Camera.main.transform.position;
		position.y = transform.position.y;

		if (m_isUseRectTransform)
		{
			var rect = GetComponent<RectTransform>();
			if (rect != null)
				rect.rotation = Quaternion.LookRotation(Camera.main.transform.position - position);
		}
		else
		{
			transform.LookAt(position);
		}
	}
}
