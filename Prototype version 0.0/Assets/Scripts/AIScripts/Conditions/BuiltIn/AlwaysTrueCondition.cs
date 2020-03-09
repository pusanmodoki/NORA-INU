//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy AI Components 
/// </summary>
namespace AIComponent
{
	/// <summary>
	///常にTrueを返す AlwaysTrueAITableCondition 
	/// </summary>
	public class AlwaysTrueAITableCondition : BaseAITableCondition
	{
		/// <summary>
		/// [IsAITableCondition]
		/// return: テーブル条件を満たしているか否か
		/// </summary>
		public override bool IsAITableCondition()
        {
            return true;
        }
    }
}