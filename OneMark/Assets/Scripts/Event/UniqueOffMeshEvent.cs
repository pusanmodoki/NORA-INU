using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UniqueOffMeshEvent : BaseEvent
{
	[SerializeField, Tooltip("Off Mesh Linkと排他")]
	NavMeshLink m_navMeshLink = null;
	[SerializeField, Tooltip("Off Mesh Linkと排他")]
	OffMeshLink m_offMeshLink= null;
	[SerializeField, Tooltip("起動するUnique Off Mesh Link")]
	BaseUniqueOffMeshLink m_uniqueOffMeshLink = null;
	
	/// <summary>This Transform.position</summary>
	Vector3 m_position = Vector3.zero;
	/// <summary>Move target</summary>
	Vector3 m_targetPosition = Vector3.zero;

	protected override void NearbyIfManualTrigger()
	{
	}

	protected override void StartEvent()
	{
		if (linkPlayerInfo.navMeshController.isOnManualUniqueOffMeshLink
			|| !m_uniqueOffMeshLink.AcquisitionRightToUse())
			return;

		//初期地点, 完了地点
		Vector3 startPoint, endPoint;

		//position取得, フラグ設定
		m_position = linkPlayerInfo.gameObject.transform.position;

		//OffMeshLink??
		if (m_navMeshLink == null)
		{
			startPoint = m_offMeshLink.startTransform.position;
			endPoint = m_offMeshLink.endTransform.position;
		}
		//NavMeshLink??
		else
		{
			startPoint = m_navMeshLink.transform.LocalToWorldPosition(m_navMeshLink.startPoint);
			endPoint = m_navMeshLink.transform.LocalToWorldPosition(m_navMeshLink.endPoint);
		}

		//距離でどっちが初期地点か判定
		if ((startPoint - m_position).sqrMagnitude > (endPoint - m_position).sqrMagnitude)
		{
			//反対から行けないなら終了
			if ((m_navMeshLink != null && !m_navMeshLink.bidirectional)
				|| (m_offMeshLink != null && !m_offMeshLink.biDirectional))
				return;

			Vector3 temp = startPoint;
			startPoint = endPoint;
			endPoint = temp;
		}
		//Set Start Point
		startPoint = m_position;

		//NavMeshLinkの場合EndPointが信用できないので自分で求める
		if (m_navMeshLink != null)
		{
			endPoint = DogOffMeshLinkController.CalculateNavMeshLinkEndPoint(
				m_navMeshLink, ref m_position, ref startPoint, ref endPoint);
			m_targetPosition.x = endPoint.x;
			m_targetPosition.z = endPoint.z;
		}

		////TargetPosition設定
		//if (NavMesh.SamplePosition(endPoint, out m_navMeshHit, 10.0f, NavMesh.AllAreas))
		//	m_targetPosition = m_navMeshHit.position;
		//else
		{
			m_targetPosition = endPoint;
			//Y調整
			m_targetPosition.y += m_position.y - startPoint.y;
		}

		if (linkPlayerInfo.navMeshController.isOnManualUniqueOffMeshLink)
			return;

		var type = m_uniqueOffMeshLink.GetType();
		var copy = linkPlayerInfo.gameObject.AddComponent(type) as BaseUniqueOffMeshLink;
		copy.CopyComponent(m_uniqueOffMeshLink, type);

		copy.Link(linkPlayerInfo.transform, m_uniqueOffMeshLink.transform,
			linkPlayerInfo.rigidBody, linkPlayerInfo.navMeshAgent, linkPlayerInfo.groundFlag,
			ref startPoint, ref endPoint, ref m_targetPosition);

		if (!linkPlayerInfo.navMeshController.LinkManualUniqueOffMeshLink(m_uniqueOffMeshLink, copy))
		{
			copy.Unlink();
			Object.Destroy(copy);
			return;
		}
	}

	protected override void EndEvent()
	{
	}

	protected override bool UpdateEvent()
	{
		return false;
	}
}
