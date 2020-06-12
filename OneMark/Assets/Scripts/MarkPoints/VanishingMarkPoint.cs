using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingMarkPoint : BaseMarkPoint
{
	[SerializeField]
	ParticleSystem[] m_particleSystems = null;
	[SerializeField]
	float m_waitingToReturnSeconds = 5.0f;

	public override bool IsMovePoint()
	{
		return false;
	}
	public override void ReactivePoint()
	{
		foreach (var e in m_particleSystems)
			if (e != null) e.Play();

		effectiveCounter = 0.0f;
		isForceLockEffectiveCounter = false;
	}

	/// <summary>
	/// [LinkPoint] (Virtual)
	/// ポイントがリンクされた際にコールバックされる関数
	/// </summary>
	public override void LinkPoint()
	{
		isInvisible = true;
	}

	public override void UnlinkPoint()
	{
		MarkPointManager.instance.RegisterTemporarilyDeactive(this, m_waitingToReturnSeconds);
	}

	public override void UpdatePoint()
	{		
		if (effectiveCounter01 >= 1.0f && isLinked)
		{
			ServantManager.instance.allServants[linkServantID].ComeBecauseEndOfMarking(true);
			effectControler.OffEffectByInteger(0);
			UnlinkPlayer();

			isInvisible = false;
			isForceLockEffectiveCounter = true;
			effectiveCounter = effectiveMaxLimiter;
		}
	}
}
