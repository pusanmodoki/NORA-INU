//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自身のオブジェクトたちの中でメインとなるオブジェクトを提供するProvideMainObject
/// </summary>
public class ProvideMainObject : MonoBehaviour
{
	/// <summary>Main object</summary>
	public GameObject mainObject { get { return m_mainObject; } }
	/// <summary>Is set main object</summary>
	public bool isSet { get { return m_mainObject != null; } }

	/// <summary>Is set main object</summary>
	[SerializeField, Tooltip("自身のオブジェクト構成の中でメインとなるオブジェクト\n　position等をよく参照するであろう…")]
	GameObject m_mainObject = null;
}
