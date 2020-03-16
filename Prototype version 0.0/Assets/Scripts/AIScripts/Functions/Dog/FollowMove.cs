using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
	public class FollowMove : BaseAIFunction
	{
		[SerializeField]
		ManageCommander m_manageCommander = null;

		public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
		{
			SetUpdatePosition(true);
		}

		public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
		{
		}

		public override void AIUpdate(UpdateIdentifier updateIdentifier)
		{
			navMeshAgent.destination = m_manageCommander.commander.transform.position;
		}
	}
}