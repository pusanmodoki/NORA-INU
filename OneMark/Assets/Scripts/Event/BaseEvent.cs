using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvent : MonoBehaviour
{
	public PlayerAndTerritoryManager.PlayerInfo linkPlayerInfo { get; private set; } = null;
	public bool isAutoTrigger { get { return m_isAutoTrigger; } } 
	public bool isLinked { get { return linkPlayerInfo != null; } }

	[SerializeField]
	bool m_isAutoTrigger = false;
	
	public void CallNearbyIfManualTrigger()
	{
		NearbyIfManualTrigger();
	}

	public bool TriggerEvent(GameObject linkPlayer)
	{
		linkPlayerInfo = PlayerAndTerritoryManager.instance.allPlayers[linkPlayer.GetInstanceID()];
		if (linkPlayerInfo.isLinkEvent)
		{
			linkPlayerInfo = null;
			return false;
		}

		linkPlayerInfo.SetLinkEvent(true);
		StartEvent();
		return true;
	}

	protected abstract void NearbyIfManualTrigger();

	protected abstract void StartEvent();

	protected abstract bool UpdateEvent();

	protected abstract void EndEvent();

    // Update is called once per frame
    void Update()
    {
        if (isLinked)
		{
			if (!UpdateEvent())
			{
				EndEvent();
				linkPlayerInfo.SetLinkEvent(false);
				linkPlayerInfo = null;
			}
		}
    }
}
