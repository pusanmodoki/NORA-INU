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
	///常にFalseを返す AlwaysFalseAITableCondition 
	/// </summary>
	public class AlwaysFalseAITableCondition : BaseAITableCondition
	{
		/// <summary>
		/// [IsAITableCondition]
		/// return: テーブル条件を満たしているか否か
		/// </summary>
		public override bool IsAITableCondition()
        {
            return false;
        }
    }
}
