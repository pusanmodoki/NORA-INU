using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingMessage : MonoBehaviour
{
	[System.Serializable]
	public struct Message
	{
		public Message(Marker marker)
		{
			this.marker = marker;
			this.attack = marker.attack;
		}

		public Marker marker;
		public float attack;
	}

	public bool isEmpty { get { return m_messages.Count == 0; } }
	public int sizeMessages { get { return m_messages.Count; } }

	[SerializeField, Tooltip("drawing messages")]
	List<Message> m_messages = new List<Message>();

	public void SendMessage(Marker marker)
	{
		m_messages.Add(new Message(marker));
	}

	public Message GetMessage(int index)
	{
		if (m_messages.Count > index)
		{
			return m_messages[index];
		}
		else
		{
			Debug.LogError("Error!! MarkingMessage->GetMessage invalid index");
			return new Message();
		}
	}
}
