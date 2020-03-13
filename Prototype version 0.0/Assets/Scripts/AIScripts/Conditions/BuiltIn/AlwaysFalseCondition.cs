﻿//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
	/// <summary>
	///常にFalseを返す AlwaysFalseCondition 
	/// </summary>
	public class AlwaysFalseCondition : BaseAICondition
	{
		/// <summary>
		/// [IsAITableCondition]
		/// return: テーブル条件を満たしているか否か
		/// </summary>
		public override bool IsCondition()
        {
            return false;
        }
    }
}
