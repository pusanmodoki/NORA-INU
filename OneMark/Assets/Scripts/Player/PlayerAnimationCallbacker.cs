using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationCallbacker : MonoBehaviour
{
	[SerializeField]
	PlayerInput m_input = null;

	public void EditEnableInput(int set)
	{
		if (set == 0) m_input.isEnableInput = false;
		else if (set == 1) m_input.isEnableInput = true;
	}
}
