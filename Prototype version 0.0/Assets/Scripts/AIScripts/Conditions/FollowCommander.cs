using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
	public class FollowCommander : BaseAICondition
	{
		[SerializeField]
		ManageCommander m_manageCommander = null;

		public override bool IsCondition()
		{
			return m_manageCommander.isLinked;
		}
	}
}