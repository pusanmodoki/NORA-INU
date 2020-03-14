//制作者: 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
	public class KamikazeMove : BaseAIFunction
	{
		static readonly Vector3 m_cRaycastUp = new Vector3(0.0f, 500.0f, 0.0f);
		static readonly float m_cRaycastDistance = 1000.0f;

		[SerializeField]
		KamikazeCommand m_kamikazeCommand = null;
		[SerializeField]
		AIVisibility m_visibility = null;
		[SerializeField]
		LayerMaskEx m_stageLayer = 0;

		bool m_isEnabledNavMesh = false;

		public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
		{
			RaycastHit raycastHit;
			Vector3 targetPosition = Vector3.zero;
			Vector3 position = transform.position;

			if (Physics.Raycast(position, m_kamikazeCommand.direction, out raycastHit, m_cRaycastDistance, m_stageLayer))
				targetPosition = raycastHit.point;
			else
				targetPosition = position + m_kamikazeCommand.direction * m_cRaycastDistance;

			if (Physics.Raycast(targetPosition + m_cRaycastUp - m_kamikazeCommand.direction, Vector3.down, out raycastHit, m_cRaycastDistance, m_stageLayer))
				targetPosition = raycastHit.point + Vector3.up;
			else
				targetPosition += Vector3.down * m_cRaycastDistance;

			m_isEnabledNavMesh = navMeshAgent.SetDestination(targetPosition);
			if (m_isEnabledNavMesh)
				SetUpdatePosition(true);
		}

		public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
		{
		}

		public override void AIUpdate(UpdateIdentifier updateIdentifier)
		{
			if (m_isEnabledNavMesh == false | m_kamikazeCommand.timeoutSeconds < timer.elapasedTime)
			{
				EndAIFunction(updateIdentifier);
				m_kamikazeCommand.EndKamikaze();
				return;
			}

			if (m_visibility.IsHitVisibility())
				aiAgent.AllocateFunction();
		}
	}
}