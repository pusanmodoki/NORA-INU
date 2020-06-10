using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingMarkPoint : BaseMarkPoint
{
	[SerializeField]
	float m_waitingToReturnSeconds = 5.0f;

	public override bool IsMovePoint()
	{
		return false;
	}

	public override void UnlinkPoint()
	{
		MarkPointManager.instance.RegisterTemporarilyDeactive(this, m_waitingToReturnSeconds);
	}

	public override void UpdatePoint()
	{		
		if (effectiveCounter01 >= 1.0f)
		{
			effectiveCounter = 0.0f;
			ServantManager.instance.allServants[linkServantID].ComeBecauseEndOfMarking(true);
			effectControler.OffEffectByInteger(0);
			UnlinkPlayer();
		}
	}
}
