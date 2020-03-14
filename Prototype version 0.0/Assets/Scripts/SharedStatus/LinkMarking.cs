//制作者: 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MarkPointとMarkerをLinkさせるLinkMarking 
/// </summary>
public class LinkMarking : MonoBehaviour
{
	/// <summary>
	/// Linkオブジェクトの情報として提供されるInfomations
	/// </summary>
	[System.Serializable]
	public class Infomations
	{
		/// <summary>Link main object</summary>
		public GameObject linkObject { get { return m_linkObject; } }
		/// <summary>コマンダーが存在する場合->Commander, 存在しない場合null</summary>
		public GameObject commander { get; private set; } = null;

		/// <summary>Link main object</summary>
		[SerializeField, Tooltip("Link main object")]
		GameObject m_linkObject = null;

		/// <summary>
		/// [SetCommander]
		/// Commanderを設定する
		/// 引数1: Commander
		/// </summary>
		public void SetCommander(GameObject commander)
		{
			this.commander = commander;
		}
	}

	/// <summary>
	/// delegate::LinkAction
	/// リンク先が変更された場合に呼び出されるコールバック形式
	/// 引数1: Callbackを登録したComponent
	/// 引数2: Link先のComponent
	/// 引数1: リンクさせる->true, リンク解除->false
	/// </summary>
	public delegate void LinkAction(LinkMarking thisComponent, LinkMarking linkedComponent, bool isLinked);

	/// <summary>Provision of infomations</summary>
	public Infomations infomations { get { return m_takeInfomations; } }
	/// <summary>Linked Component</summary>
	public LinkMarking linked { get; private set; } = null;
	/// <summary>linked != null?</summary>
	public bool isLink { get { return linked != null;  } }

	/// <summary>Commanderを登録する(オス犬の)場合は参照させてください</summary>
	[SerializeField, Tooltip("Commanderを登録する(オス犬の)場合は参照させてください")]
	ManageCommander m_manageCommander = null;
	/// <summary>リンク先で提供される情報</summary>
	[SerializeField, Tooltip("リンク先で提供される情報")]
	Infomations m_takeInfomations = null;

	///<summary>Callback</summary>
	LinkAction m_linkNotifyCallback = null;

	/// <summary>
	/// [RegisterLinkNotifyCallbback]
	/// リンク先が変更された際に呼び出されるCallbackを登録する
	/// 引数1: 登録するCallback
	/// </summary>
	public void RegisterLinkNotifyCallbback(LinkAction callback)
	{
		if (callback != null)
			m_linkNotifyCallback += callback;
	}

	/// <summary>
	/// [SettingLink]<static>
	/// リンク設定する
	/// 引数1: Link left
	/// 引数1: Link right
	/// </summary>
	public static void SettingLink(LinkMarking left, LinkMarking right)
	{
		if (left == null || right == null) return;
		if (left.isLink) ReleaseLink(left);
		if (right.isLink) ReleaseLink(right);

		//リンク設定
		left.linked = right;
		right.linked = left;
		//コールバック呼び出し
		left.m_linkNotifyCallback?.Invoke(left, right, true);
		right.m_linkNotifyCallback?.Invoke(right, left, true);
	}

	/// <summary>
	/// [ReleaseLink]<static>
	/// リンク解除する
	/// 引数1: Linkしているどちらかのコンポーネント
	/// </summary>
	public static void ReleaseLink(LinkMarking either)
	{
		if (either == null | !either.isLink) return;

		//コールバック呼び出し
		either.m_linkNotifyCallback?.Invoke(either, either.linked, false);
		either.linked.m_linkNotifyCallback?.Invoke(either.linked, either, false);

		//リンク先をnullに設定
		either.linked.linked = null;
		either.linked = null;
	}

	/// <summary>[Awake]</summary>
	void Awake()
	{
		//Callback登録
		if (m_manageCommander != null)
			m_manageCommander.RegisterLinkNotifyCallbback(this.ActionLinkCommander);
	}

	/// <summary>
	/// [ActionLinkCommander]
	/// コマンダーが変更された際に呼び出させるコールバック
	/// 引数1: コマンダー
	/// 引数2: 登録->true, 解除->false
	/// </summary>
	void ActionLinkCommander(GameObject commander, bool isLinked)
	{
		m_takeInfomations.SetCommander(isLink ? commander : null);
	}
}
