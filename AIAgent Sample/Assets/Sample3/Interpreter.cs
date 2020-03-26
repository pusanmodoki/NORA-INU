using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
	[SerializeField, Tooltip("AI Agent")]
	AIComponent.AIAgent m_aiAgent = null;
	[SerializeField, Tooltip("Push A!! function")]
	AIComponent.BaseAIFunction m_pushA = null;
	[SerializeField, Tooltip("Push D!! function")]
	AIComponent.BaseAIFunction m_pushD = null;

    // Update is called once per frame
    void Update()
    {
		//A押したら割り込み実行
		//今回は外部からしているが、関数側からも実行可能
		if (Input.GetKeyDown(KeyCode.A))
			m_aiAgent.ForceSpecifyFunction(m_pushA);

		//D押したら割り込み実行
		//今回は外部からしているが、関数側からも実行可能
		if (Input.GetKeyDown(KeyCode.D))
			m_aiAgent.ForceSpecifyFunction(m_pushD);
	}
}
