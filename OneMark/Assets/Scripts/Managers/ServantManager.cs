using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// オスくんを管理するServantManager
/// </summary>
[DefaultExecutionOrder(-100)]
public class ServantManager : MonoBehaviour
{
	/// <summary>Static instance</summary>
	public static ServantManager instance { get; private set; } = null;

	/// <summary>MainPlayerに従うservants</summary>
	public ReadOnlyCollection<DogAIAgent> servantByMainPlayer { get { return m_servantByMainPlayer != null? m_servantByMainPlayer.AsReadOnly() : null; } }

	/// <summary>Manage servants</summary>
	Dictionary<int, DogAIAgent> m_servants = new Dictionary<int, DogAIAgent>();

	/// <summary>Player別servants</summary>
	Dictionary<int, List<DogAIAgent>> m_servantByPlayers = new Dictionary<int, List<DogAIAgent>>();
	/// <summary>MainPlayerに従うservants</summary>
	List<DogAIAgent> m_servantByMainPlayer = null;

	/// <summary>[Awake]</summary>
	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// [AddPlayer]
	/// Playerを登録する
	/// 引数1: GameObject.GetInstanceID()
	/// 引数2: This main player?, default = true
	/// </summary>
	public void AddPlayer(int instanceID, bool isMainPlayer = true)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (m_servantByPlayers.ContainsKey(instanceID))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->AddPlayer\n ContainsKey(instanceID) == true");
			return;
		}
#endif

		m_servantByPlayers.Add(instanceID, new List<DogAIAgent>());
		if (isMainPlayer)
			m_servantByMainPlayer = m_servantByPlayers[instanceID];
	}
	/// <summary>
	/// [RemovePlayer]
	/// DogAIAgentを登録解除する
	/// 引数1: GameObject.GetInstanceID()
	/// </summary>
	public void RemovePlayer(int instanceID)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_servantByPlayers.ContainsKey(instanceID))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->RemovePlayer\n ContainsKey(instanceID) == false");
			return;
		}
#endif

		if (m_servantByPlayers[instanceID] == m_servantByMainPlayer)
			m_servantByMainPlayer = null;
		m_servantByPlayers.Remove(instanceID);
	}

	/// <summary>
	/// [AddServant]
	/// DogAIAgentを登録する
	/// 引数1: DogAIAgent
	/// </summary>
	public void AddServant(DogAIAgent dogAgent)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (m_servants.ContainsKey(dogAgent.aiAgentInstanceID))
		{
			Debug.LogError("Error!! ServantManager->AddServant\n ContainsKey(instanceID) == true");
			return;
		}
#endif

		m_servants.Add(dogAgent.aiAgentInstanceID, dogAgent);
	}
	/// <summary>
	/// [RemoveServant]
	/// DogAIAgentを登録解除する
	/// 引数1: DogAIAgent
	/// </summary>
	public void RemoveServant(DogAIAgent dogAgent)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_servants.ContainsKey(dogAgent.aiAgentInstanceID))
		{
			Debug.LogError("Error!! ServantManager->RemoveServant\n ContainsKey(instanceID) == false");
			return;
		}
#endif

		m_servants.Remove(dogAgent.aiAgentInstanceID);
	}

	/// <summary>
	/// [RegisterPlayerOfServant]
	/// DogAIAgentとPlayerを紐付けする
	/// 引数1: DogAIAgent
	/// 引数2: PlayerObject
	/// </summary>
	public void RegisterPlayerOfServant(DogAIAgent dogAgent, GameObject player)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_servants.ContainsKey(dogAgent.aiAgentInstanceID))
		{
			Debug.LogError("Error!! ServantManager->RegisterPlayerOfServant\n ContainsKey(instanceID) == false");
			return;
		}
		if (!m_servantByPlayers.ContainsKey(player.GetInstanceID()))
		{
			Debug.LogError("Error!! ServantManager->RegisterPlayerOfServant\n ContainsKey(instanceID) == false");
			return;
		}
#endif

		m_servantByPlayers[player.GetInstanceID()].Add(dogAgent);
	}
	/// <summary>
	/// [UnregisterPlayerOfServant]
	/// DogAIAgentとPlayerを紐付け解除する
	/// 引数1: DogAIAgent
	/// 引数2: PlayerObject
	/// </summary>
	public void UnregisterPlayerOfServant(DogAIAgent dogAgent, GameObject player)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_servants.ContainsKey(dogAgent.aiAgentInstanceID))
		{
			Debug.LogError("Error!! ServantManager->RegisterPlayerOfServant\n ContainsKey(instanceID) == false");
			return;
		}
		if (!m_servantByPlayers.ContainsKey(player.GetInstanceID()))
		{
			Debug.LogError("Error!! ServantManager->RegisterPlayerOfServant\n ContainsKey(instanceID) == false");
			return;
		}
#endif

		m_servantByPlayers[player.GetInstanceID()].Remove(dogAgent);
	}

	/// <summary>
	/// [GetServant]
	/// DogAIAgentを取得する
	/// 引数1: DogAIAgent.aiAgentInstanceID
	/// </summary>
	public DogAIAgent GetServant(int instanceID)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_servants.ContainsKey(instanceID))
		{
			Debug.LogError("Error!! ServantManager->GetServant\n ContainsKey(instanceID) == false");

			if (m_servants.Count > 0)
			{
				var iterator = m_servants.GetEnumerator();
				iterator.MoveNext();
				return iterator.Current.Value;
			}
			else return null;
		}
#endif

		return m_servants[instanceID];
	}
}
