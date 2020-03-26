using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCommanderLink : MonoBehaviour
{
	ServantTerritoryCollider m_territoryCollider = null;
	ManageCommander m_manageCommander = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (m_territoryCollider.isEnter)
		{

		}
		else if (m_territoryCollider.isExit)
		{

		}
    }
}
