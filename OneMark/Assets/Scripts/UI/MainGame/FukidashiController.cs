using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FukidashiController : MonoBehaviour
{
	int m_cPlayID = Animator.StringToHash("Play");
	int m_cExitID = Animator.StringToHash("Exit");
	public bool isEnableEffect { get; private set; } = false; 

	[SerializeField]
	Animator m_animator = null;
	
	public void EnableEffect()
	{
		if (isEnableEffect) return;

		m_animator.SetTrigger(m_cPlayID);

		isEnableEffect = true;
	}

	public void DisableEffect()
	{
		if (!isEnableEffect) return;

		isEnableEffect = false;
		m_animator.SetTrigger(m_cExitID);
	}
}
