using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIComponent;

/// <summary>
/// WaitAndRunしているかを判断するDogWaitAndRunCondition
/// </summary>
public class DogWaitAndRunCondition : BaseAICondition
{
	/// <summary>This DogAIAgent</summary>
	[SerializeField, Tooltip("This DogAIAgent")]
	DogAIAgent m_dogAIAgent = null;

	/// <summary>
	/// [IsCondition]
	/// return: テーブル条件を満たしているか否か
	/// </summary>
	public override bool IsCondition()
	{
		return m_dogAIAgent != null && (m_dogAIAgent.isLinkPlayer 
			& m_dogAIAgent.isLinkMarkPoint & m_dogAIAgent.isWaitAndRunSelf);
	}
}
