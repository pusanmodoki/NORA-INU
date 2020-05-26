using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更新を行わないデフォルトクラスとなるDefaultMarkPoint 
/// </summary>
public class DefaultMarkPoint : BaseMarkPoint
{
	[SerializeField]
	GameObject m_uiObject = null;
	[SerializeField]
	UnityEngine.UI.Slider m_uiSlider = null;
    [SerializeField]
    EffectControler effects = null;

	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
   //     if (isLinked)
			//m_uiSlider.value = effectiveCounter / MarkPointManager.instance.effectiveMaxLimiter;

        if (isForceAscendingEffective && effectiveCounter < MarkPointManager.instance.effectiveMaxLimiter)
        {
            ParticleSystem.EmissionModule emission = effects.GetParticleSystem("flower").emission;

            emission.rateOverTime = 11.0f;
        }
        else
        {
            ParticleSystem.EmissionModule emission = effects.GetParticleSystem("flower").emission;

            emission.rateOverTime = 0.0f;
        }
    }

    /// <summary>
    /// [LinkPoint] (Virtual)
    /// ポイントがリンクされた際にコールバックされる関数
    /// </summary>
    public override void LinkPoint()
	{
		//m_uiObject.SetActive(true);
        effects.OnEffectByInteger(0);
	}
	/// <summary>
	/// [UnlinkPoint] (Virtual)
	/// ポイントがリンク解除された際にコールバックされる関数
	/// </summary>
	public override void UnlinkPoint()
	{
		//m_uiObject.SetActive(false);
	}
}
