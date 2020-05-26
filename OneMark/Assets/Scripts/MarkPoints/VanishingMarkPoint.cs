using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingMarkPoint : BaseMarkPoint
{
	public override void UpdatePoint()
	{

	}

	/// <summary>
	/// [LinkAscendingPerSeconds] (Virtual)
	/// return: リンク時のゲージ上昇速度 per seconds
	/// </summary>
	public override float LinkAscendingPerSeconds() { return 1.0f; }
	/// <summary>
	/// [UnlinkDecreasingPerSeconds] (Virtual)
	/// return: リンク解消時のゲージ減少速度 per seconds
	/// </summary>
	public override float UnlinkDecreasingPerSeconds() { return 1.0f; }
	/// <summary>
	/// [EffectiveMaxLimiter] (Virtual)
	/// return: マーキング最大時間
	/// </summary>
	public override float EffectiveMaxLimiter() { return 7.5f; }
	/// <summary>
	/// [EffectiveMaxLimiter] (Virtual)
	/// return: 初回リンク時のカウンターボーナス割合 (0.0f ~ 1.0f)
	/// </summary>
	protected override float EffectiveFirstLinkBonusRatio() { return 0.5f; }
	/// <summary>
	/// [IsJoinSafetyAreaWhenLink] (Virtual)
	/// return: リンク中Safetyエリアに加えるか？
	/// </summary>
	public override bool IsJoinSafetyAreaWhenLink() { return false; }
}
