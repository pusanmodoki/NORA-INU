using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingObject : MonoBehaviour
{
	[SerializeField]
	MarkingMessage m_message = null;
	[SerializeField]
	float m_hp = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
		if (m_message == null)
		{
			Debug.LogError("Error!! MarkingObject->Awake message reference == null");
			return;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_message.isEmpty && m_hp > 0.0f)
		{
			for(int i = 0; i < m_message.sizeMessages; ++i)
			{
				var message = m_message.GetMessage(i);
				m_hp -= message.attack;
			}

			if (m_hp <= 0.0f)
				Debug.Log("Death....");
		}
    }
}
