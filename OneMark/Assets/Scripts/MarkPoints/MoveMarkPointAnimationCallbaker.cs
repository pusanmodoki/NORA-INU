using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarkPointAnimationCallbaker : MonoBehaviour
{
	[SerializeField]
	MoveMarkPoint m_markPoint = null;

	public void AnimationEscapeCallback()
	{
		m_markPoint.AnimationEscapeCallback();
	}
}
