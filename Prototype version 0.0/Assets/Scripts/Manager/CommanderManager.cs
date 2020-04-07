using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderManager : MonoBehaviour
{
	public struct Commander
	{
		public Commander(GameObject gameObject)
		{
			this.gameObject = gameObject;
			transform = gameObject.transform;
			manageServants = gameObject.GetComponent<ManageServants>();
			manageTerritory = gameObject.GetComponent<ManageTerritory>();
		}

		public bool isValid { get { return gameObject != null; } }
		public GameObject gameObject;
		public Transform transform;
		public ManageServants manageServants;
		public ManageTerritory manageTerritory;
	}

	public static CommanderManager instance { get; private set; } = null;
	public int commanderCount { get { return m_commanders.Count; } }

	Dictionary<int, Commander> m_commanders = new Dictionary<int, Commander>();

	public void RegisterCommander(GameObject gameObject)
	{
		m_commanders.Add(gameObject.GetInstanceID(), new Commander(gameObject));
	}

	public void UnregisterCommander(GameObject gameObject)
	{
		m_commanders.Remove(gameObject.GetInstanceID());
	}

	public Commander GetCommanderName(string name)
	{
		foreach(var e in m_commanders)
		{
			if (e.Value.gameObject.name == name)
				return e.Value;
		}
		return default;
	}
	public Commander GetCommanderObject(GameObject gameObject)
	{
		foreach (var e in m_commanders)
		{
			if (e.Value.gameObject.GetInstanceID() == gameObject.GetInstanceID())
				return e.Value;
		}
		return default;
	}
	public Commander GetCommanderIndex(int index)
	{
		int i = 0;
		foreach (var e in m_commanders)
		{
			if (i == index) return e.Value;
			++i;
		}
		return default;
	}

	void Awake()
	{
		if (instance == null) instance = this;
	}
	void OnDestroy()
	{
		if (instance == this) instance = this;
	}
}
