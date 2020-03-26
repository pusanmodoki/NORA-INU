using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTerritoryCollider : BaseTerritoryCollider
{
	[SerializeField, Space]
	ManageTerritory m_manageTerritory = null;

    // Update is called once per frame
    void LateUpdate()
    {
		if (!isDetectionFrame || m_manageTerritory.territoryPoints == null) return;

		SetHitFlags(CollisionTerritory.HitLineTerritory(m_manageTerritory.territoryPoints, transform.position, m_cHitDistance, -m_radius));
	}
}
