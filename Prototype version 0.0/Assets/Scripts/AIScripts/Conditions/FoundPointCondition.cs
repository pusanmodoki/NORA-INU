//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
	/// <summary>
	/// 見つけたらtrueを返すFoundPointCondition
	/// </summary>
	public class FoundPointCondition : BaseAICondition
	{
		///<summary>This visibility</summary>
		[SerializeField, Tooltip("This visibility")]
		AIVisibility m_visibility = null;
		[SerializeField, Tooltip("This kamikaze command")]
		KamikazeCommand m_kamikazeCommand = null;

		/// <summary>
		/// [IsCondition]
		/// return: テーブル条件を満たしているか否か
		/// </summary>
		public override bool IsCondition()
		{
			return m_kamikazeCommand.isKamikazeNow && m_visibility.IsHitVisibility() && m_visibility.lookTarget != null;
		}
	}
}