using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarkPoint : BaseMarkPoint
{
	public int nowUseAgent { get; private set; } = 0;

	[SerializeField]
	GameObject m_alphaUiObject = null;
	[SerializeField]
	UnityEngine.UI.Slider m_alphaUiSlider = null;
	[SerializeField]
	float m_checkOverlapInterval = 0.1f;
	[SerializeField]
	BoxCastInfos m_checkOverlapInfo = default;

	[SerializeField, Tooltip("1度に使用できる最大エージェント数, infinity = -1")]
	int m_maxAgentToUse = 1;
#if UNITY_EDITOR
	[SerializeField, Header("Debug Only"), Tooltip("現在のエージェント数")]
	int m_dNowUseAgent = 0;
#endif

	List<DogAIAgent> m_useAgents = new List<DogAIAgent>();
	Timer m_intervalTimer = new Timer();

	public bool AcquisitionRightToUse(DogAIAgent useObject)
	{
		if (nowUseAgent < (m_maxAgentToUse >= 0 ? m_maxAgentToUse : int.MaxValue)
			&& !m_useAgents.Contains(useObject))
		{
			++nowUseAgent;
			m_useAgents.Add(useObject);

#if UNITY_EDITOR
			m_dNowUseAgent = nowUseAgent;
#endif
			return true;
		}
		else
			return false;
	}

	public void UnacquisitionRightToUse(DogAIAgent useObject)
	{
		if (m_useAgents.Contains(useObject))
			return;

		--nowUseAgent;
		m_useAgents.Remove(useObject);

#if UNITY_EDITOR
		m_dNowUseAgent = nowUseAgent;
#endif
	}

	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public override void UpdatePoint()
	{
		if (isLinked)
			m_alphaUiSlider.value = effectiveCounter / MarkPointManager.instance.effectiveMaxLimiter;

		if (m_intervalTimer.elapasedTime > m_checkOverlapInterval)
		{
			var collisions = Physics.OverlapBox(m_checkOverlapInfo.WorldCenter(transform), m_checkOverlapInfo.overlapSize);

			for (int i = 0, length = collisions.Length; i < length; ++i)
				for (int k = 0, count = m_useAgents.Count; k < count; ++k)
				{
					if (collisions[i].gameObject.GetInstanceID() == m_useAgents[k].gameObject.GetInstanceID())
					{

					}
				}

			m_intervalTimer.Start();
		}
	}

	/// <summary>
	/// [LinkPoint] (Virtual)
	/// ポイントがリンクされた際にコールバックされる関数
	/// </summary>
	public override void LinkPoint()
	{
		m_alphaUiObject.SetActive(true);
		m_intervalTimer.Start();
	}
	/// <summary>
	/// [UnlinkPoint] (Virtual)
	/// ポイントがリンク解除された際にコールバックされる関数
	/// </summary>
	public override void UnlinkPoint()
	{
		m_alphaUiObject.SetActive(false);
	}

	/// <summary>
	/// [LinkAscendingPerSeconds] (Virtual)
	/// return: リンク時のゲージ上昇速度 per seconds
	/// </summary>
	public override float LinkAscendingPerSeconds()
	{
		return 1.0f;
	}
	/// <summary>
	/// [UnlinkDecreasingPerSeconds] (Virtual)
	/// return: リンク解消時のゲージ減少速度 per seconds
	/// </summary>
	public override float UnlinkDecreasingPerSeconds()
	{
		return 1.0f;
	}
	/// <summary>
	/// [EffectiveMaxLimiter] (Virtual)
	/// return: マーキング最大時間
	/// </summary>
	public override float EffectiveMaxLimiter()
	{
		return 7.5f;
	}
	/// <summary>
	/// [EffectiveMaxLimiter] (Virtual)
	/// return: 初回リンク時のカウンターボーナス割合 (0.0f ~ 1.0f)
	/// </summary>
	protected override float EffectiveFirstLinkBonusRatio()
	{
		return 0.5f;
	}
	/// <summary>
	/// [IsJoinSafetyAreaWhenLink] (Virtual)
	/// return: リンク中Safetyエリアに加えるか？
	/// </summary>
	public override bool IsJoinSafetyAreaWhenLink()
	{
		return true;
	}
}
