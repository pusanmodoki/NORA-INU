using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIComponent;

/// <summary>
/// ABaseAIFunctionをCustomしたDogAIFunction
/// </summary>
public abstract class BaseDogAIFunction : AIComponent.BaseAIFunction
{
	/// <summary> Dog ai agent </summary>
	public DogAIAgent dogAIAgent { get; private set; } = null;

	/// <summary>
	/// [StartAIFunction]
	/// AIAgentで呼び出すStart関数
	/// 引数1: 自身の所属するAIAgent
	/// 引数2: 自身の所属するAITable
	/// </summary>
	public override void StartAIFunction(AIAgent aiAgent, AITable aiTable)
	{
		base.StartAIFunction(aiAgent, aiTable);
		dogAIAgent = aiAgent as DogAIAgent;

#if UNITY_EDITOR
		if (dogAIAgent == null)
			Debug.LogError("Error!! DogAIFunction->StartAIFunction\n AIAgent->DogAIAgent cast failed");
#endif
	}
}
