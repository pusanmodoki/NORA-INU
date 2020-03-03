using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
	public float attack { get { return m_attack; } }

	[SerializeField]
	float m_attack = 1.0f;

	void OnTriggerEnter(Collider collision)
	{
		var message = collision.gameObject.GetComponentInParent<MarkingMessage>();

		if (message != null)
			message.SendMessage(this);
	}
}
