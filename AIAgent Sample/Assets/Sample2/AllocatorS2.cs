using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllocatorS2 : MonoBehaviour
{
	[SerializeField, Tooltip("AIAgent")]
	AIComponent.AIAgent m_aiAgent = null;

    // Update is called once per frame
    void Update()
    {
		//押したら再割当て
		//今回は外部からしているが、関数側からも実行可能
		if (Input.GetKeyDown(KeyCode.Space))
			m_aiAgent.AllocateFunction();
	}
}
