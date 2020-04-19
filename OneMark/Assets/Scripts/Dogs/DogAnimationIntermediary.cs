using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationIntermediary : MonoBehaviour
{
	[SerializeField]
	DogAnimationController m_animationController = null;

	void AnimationWakeUpCallback()
	{
		m_animationController.AnimationWakeUpCallback();
	}
}
