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

	bool m_isLink = false;

	public bool IsHitTerritoryEndPoint(List<Vector3> terittoryArea, Vector3 position)
	{
		//初期地点, 完了地点
		Vector3 startPoint, endPoint;

		if (m_navMeshLink != null)
		{
			startPoint = m_navMeshLink.transform.LocalToWorldPosition(m_navMeshLink.startPoint);
			endPoint = m_navMeshLink.transform.LocalToWorldPosition(m_navMeshLink.endPoint);

			//endPointの線分に沿ったVector
			Vector3 right = Vector3.Cross(new Vector3(endPoint.x - startPoint.x, 0.0f, endPoint.z - startPoint.z).normalized, Vector3.up);
			//endPoint segment start
			Vector3 start = (endPoint + -right * m_navMeshLink.width * 0.5f);
			//endPoint segment end
			Vector3 end = (endPoint + right * m_navMeshLink.width * 0.5f);

			return CollisionTerritory.HitSegmentTerritory(terittoryArea, start, end);
		}
		else
		{
			if (!CalculateStartAndEndPoint(ref position, out startPoint, out endPoint))
				return false;

			return CollisionTerritory.HitCircleTerritory(terittoryArea, endPoint, Vector3.right, 1.0f);
		}
	}

	protected override void NearbyIfManualTrigger()
	{
	}

	protected override void StartEvent()
	{
		if (linkPlayerInfo.navMeshController.isOnManualUniqueOffMeshLink
			|| !m_uniqueOffMeshLink.AcquisitionRightToUse())
			return;

		//現在座標, 目標座標, 初期地点, 完了地点
		Vector3 position, targetPosition, startPoint, endPoint;

		//position取得
		position = linkPlayerInfo.gameObject.transform.position;

		if (!CalculateStartAndEndPoint(ref position, out startPoint, out endPoint))
			return;

		////TargetPosition設定
		//if (NavMesh.SamplePosition(endPoint, out m_navMeshHit, 10.0f, NavMesh.AllAreas))
		//	m_targetPosition = m_navMeshHit.position;
		//else
		{
			targetPosition = endPoint;
			//Y調整
			targetPosition.y += position.y - startPoint.y;
		}

		if (linkPlayerInfo.navMeshController.isOnManualUniqueOffMeshLink)
			return;

		var type = m_uniqueOffMeshLink.GetType();
		var copy = linkPlayerInfo.gameObject.AddComponent(type) as BaseUniqueOffMeshLink;
		copy.CopyComponent(m_uniqueOffMeshLink, type);

		//Set Start Point
		startPoint = position;

		copy.Link(linkPlayerInfo.transform, m_uniqueOffMeshLink.transform,
			linkPlayerInfo.rigidBody, linkPlayerInfo.navMeshAgent, linkPlayerInfo.groundFlag,
			ref startPoint, ref endPoint, ref targetPosition);

		if (!linkPlayerInfo.navMeshController.LinkManualUniqueOffMeshLink(m_uniqueOffMeshLink, copy))
		{
			copy.Unlink();
			Object.Destroy(copy);
			return;
		}
	}

	protected override void EndEvent()
	{
		m_isLink = false;
	}

	protected override bool UpdateEvent()
	{
		if (!m_isLink && linkPlayerInfo.navMeshController.isOnManualUniqueOffMeshLink)
			m_isLink = true;

		if (m_isLink && !linkPlayerInfo.navMeshController.isOnManualUniqueOffMeshLink)
			return false;
		else
			return true;
	}

	bool CalculateStartAndEndPoint(ref Vector3 position, out Vector3 startPoint, out Vector3 endPoint)
	{   
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

		//距離で終了地点が反対方向か判定
		if ((startPoint - position).sqrMagnitude > (endPoint - position).sqrMagnitude)
		{
			//反対から行けないなら終了
			if ((m_navMeshLink != null && !m_navMeshLink.bidirectional)
				|| (m_offMeshLink != null && !m_offMeshLink.biDirectional))
				return false;

			Vector3 temp = startPoint;
			startPoint = endPoint;
			endPoint = temp;
		}

		//NavMeshLinkの場合EndPointが信用できないので自分で求める
		if (m_navMeshLink != null)
		{
			endPoint = DogOffMeshLinkController.CalculateNavMeshLinkEndPoint(
				m_navMeshLink, ref position, ref startPoint, ref endPoint);
		}

		return true;
	}
}
