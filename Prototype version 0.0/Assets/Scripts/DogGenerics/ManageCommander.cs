//制作者: 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コマンダー情報を管理するManageCommander
/// </summary>
public class ManageCommander : MonoBehaviour
{
	/// <summary>
	/// delegate::LinkAction
	/// コマンダーが変更された場合に呼び出されるコールバック形式
	/// 引数1: コマンダー
	/// 引数2: 登録->true, 解除->false
	/// </summary>
	public delegate void LinkAction(GameObject commander, bool isLinked);

	/// <summary>Now commander</summary>
	public GameObject commander { get; private set; } = null;
	/// <summary>commander != null?</summary>
	public bool isLinked { get { return commander != null; } }

	/// <summary>Callback</summary>
	LinkAction m_linkNotifyCallback = null;

	/// <summary>
	/// [RegisterCommander]
	/// コマンダーを登録する
	/// 引数1: コマンダーのメインオブジェクト
	/// </summary>
	public void RegisterCommander(GameObject commanderObject)
	{
		//すでにコマンダーが居た場合解除する
		ReleaseCommander();

		//登録オブジェクトが存在する場合登録 & コールバック呼び出し
		if (commanderObject != null)
		{
			commander = commanderObject;
			m_linkNotifyCallback?.Invoke(commander, true);
		}
	}
	/// <summary>
	/// [ReleaseCommander]
	/// コマンダーを登録解除する
	/// </summary>
	public void ReleaseCommander()
	{
		//登録オブジェクトが存在する場合登録解除 & コールバック呼び出し
		if (commander != null)
		{
			m_linkNotifyCallback?.Invoke(commander, false);
			commander = null;
		}
	}

	/// <summary>
	/// [RegisterLinkNotifyCallbback]
	/// コマンダーが変更された場合に呼び出されるコールバックを登録する
	/// 引数1: 登録するコールバック
	/// </summary>
	public void RegisterLinkNotifyCallbback(LinkAction callback)
	{
		if (callback != null)
			m_linkNotifyCallback += callback;
	}
}
