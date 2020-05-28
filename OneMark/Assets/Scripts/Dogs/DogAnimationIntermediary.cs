using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationIntermediary : MonoBehaviour
{
	[SerializeField]
	DogAnimationController m_animationController = null;

	void AnimationMarkingEndCallback()
	{
		m_animationController.AnimationMarkingEndCallback();
	}
	void AnimationWakeUpCallback()
	{
		m_animationController.AnimationWakeUpCallback();
	}
	void AnimationChangeStoppedCallback(int set)
	{
		if (set == 0)
			m_animationController.aiAgent.navMeshAgent.isStopped = false;
		else if (set == 1)
			m_animationController.aiAgent.navMeshAgent.isStopped = true;
	}
}
