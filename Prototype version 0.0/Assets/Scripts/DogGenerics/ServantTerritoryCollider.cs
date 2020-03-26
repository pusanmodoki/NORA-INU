using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantTerritoryCollider : BaseTerritoryCollider
{
	[SerializeField, Space]
	ManageCommander m_manageCommander = null;

	public CommanderManager.Commander hitCommander { get; private set; } = default;

	bool m_isLinkedMode = false;

	void LateUpdate()
	{
		if (!isDetectionFrame || m_manageCommander == null)
			return;

		if (m_manageCommander.isLinked)
		{
			if (!m_isLinkedMode)
			{
				hitCommander = CommanderManager.instance.GetCommanderObject(m_manageCommander.gameObject);
				m_isLinkedMode = true;
			}

			SetHitFlags(CollisionTerritory.HitLineTerritory(
				m_manageCommander.manageTerritory.territoryPoints, transform.position, m_cHitDistance, -m_radius));
		}
		else
		{
			m_isLinkedMode = false;
			hitCommander = default;

			for (int i = 0; i < CommanderManager.instance.commanderCount; ++i)
			{
				var commander = CommanderManager.instance.GetCommanderIndex(i);

				if (commander.isValid)
				{
					bool result = CollisionTerritory.HitLineTerritory(
								m_manageCommander.manageTerritory.territoryPoints, transform.position, m_cHitDistance, -m_radius);

					if (result)
					{
						SetHitFlags(result);
						hitCommander = CommanderManager.instance.GetCommanderObject(m_manageCommander.gameObject);
						break;
					}
				}
			}

			if (!hitCommander.isValid)
				SetHitFlags(false);
		}
	}
}
