using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionTerritory
{
	static Vector3 m_lineStart1 = Vector3.zero;
	static Vector3 m_lineStart1ToVec2 = Vector3.zero;
	static Vector3 m_lineEnd1 = Vector3.zero;
	static float m_radius = 0.0f;

	public static bool HitCircleAndRayTerritory(List<Vector3> territoryArea, Vector3 position,
		Vector3 direction, out Vector3 normal, float circleRadius, float rayDistance, out bool isHitBody)
	{
		isHitBody = false;

		if (territoryArea.Count <= 1)
		{
			normal = Vector3.zero;
			return false;
		}

		Vector3 shortestHitPoint = position * (rayDistance * 2.0f), hitPoint;
		float sqrShortestDistance = 1000.0f;
		int hitCount = 0, bodyHitIndex0 = -1, bodyHitIndex1 = -1;
		bool isFirstHitSegment = false, isOldHitSegment = false;

		m_lineStart1 = position;
		m_lineStart1ToVec2.Set(position.x, 0.0f, position.z);
		m_lineEnd1.Set(position.x + direction.x * rayDistance, 0.0f, position.z + direction.z * rayDistance);
		m_radius = circleRadius;

		{
			if (HitCircleSegment(territoryArea[0], territoryArea[1]))
				isHitBody = true;
			
			if (HitSegments(territoryArea[0], territoryArea[1], out hitPoint))
			{
				shortestHitPoint = hitPoint;
				sqrShortestDistance = (hitPoint - position).sqrMagnitude;
				bodyHitIndex0 = 1;
				bodyHitIndex1 = bodyHitIndex0 + 1;

				++hitCount;
				isOldHitSegment = true;
				isFirstHitSegment = true;
			}
		}

		for (int i = 1, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (!isHitBody && HitCircleSegment(territoryArea[i], territoryArea[i + 1]))
				isHitBody = true;

			if (isOldHitSegment && HitPointSegment(territoryArea[i]))
			{
				isOldHitSegment = false;
				continue;
			}

			if (HitSegments(territoryArea[i], territoryArea[i + 1], out hitPoint))
			{
				float sqrDistance = (hitPoint - position).sqrMagnitude;
				if (sqrShortestDistance > sqrDistance)
				{
					shortestHitPoint = hitPoint;
					sqrShortestDistance = sqrDistance;
					bodyHitIndex0 = i;
					bodyHitIndex1 = bodyHitIndex0 + 1;
				}
				++hitCount;
				isOldHitSegment = true;
			}
			else
				isOldHitSegment = false;
		}

		{
			if (!isHitBody && HitCircleSegment(territoryArea[territoryArea.Count - 1], territoryArea[0]))
				isHitBody = true;

			if (!(isOldHitSegment && HitPointSegment(territoryArea[territoryArea.Count - 1]))
				&& !(isFirstHitSegment && HitPointSegment(territoryArea[0]))
				&& HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0], out hitPoint))
			{
				float sqrDistance = (hitPoint - position).sqrMagnitude;
				if (sqrShortestDistance > sqrDistance)
				{
					shortestHitPoint = hitPoint;
					sqrShortestDistance = sqrDistance;
					bodyHitIndex0 = territoryArea.Count - 1;
					bodyHitIndex1 = 0;
				}
				++hitCount;
			}
		}

		normal = Vector3.zero;
		m_radius *= 1.5f;
		if (bodyHitIndex1 != -1 && sqrShortestDistance < m_radius * m_radius)
			normal = Vector3.Cross(
				(territoryArea[bodyHitIndex1] - territoryArea[bodyHitIndex0]).normalized, Vector3.up).normalized;
		else
			normal = Vector3.zero;

		return isHitBody | hitCount % 2 == 1;
	}

	public static bool HitCircleTerritory(List<Vector3> territoryArea, Vector3 position,
		Vector3 direction, float circleRadius, float rayDistance = 1000.0f)
	{
		if (territoryArea.Count <= 1) return false;

		int hitCount = 0;
		bool isFirstHitSegment = false, isOldHitSegment = false;

		m_lineStart1 = position;
		m_lineStart1ToVec2.Set(position.x, 0.0f, position.z);
		m_lineEnd1.Set(position.x + direction.x * rayDistance, 0.0f, position.z + direction.z * rayDistance);
		m_radius = circleRadius;

		{
			if (HitCircleSegment(territoryArea[0], territoryArea[1]))
				return true;

			if (HitSegments(territoryArea[0], territoryArea[1]))
			{
				++hitCount;
				isOldHitSegment = true;
				isFirstHitSegment = true;
			}
		}

		for (int i = 1, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (HitCircleSegment(territoryArea[i], territoryArea[i + 1]))
				return true;

			if (isOldHitSegment && HitPointSegment(territoryArea[i]))
			{
				isOldHitSegment = false;
				continue;
			}

			if (HitSegments(territoryArea[i], territoryArea[i + 1]))
			{
				++hitCount;
				isOldHitSegment = true;
			}
			else
				isOldHitSegment = false;
		}

		{
			if (HitCircleSegment(territoryArea[territoryArea.Count - 1], territoryArea[0]))
				return true;

			if (!(isOldHitSegment && HitPointSegment(territoryArea[territoryArea.Count - 1]))
				&& !(isFirstHitSegment && HitPointSegment(territoryArea[0]))
				&& HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0]))
			{
				++hitCount;
			}
		}

		return hitCount % 2 == 1;
	}

	public static bool HitRayTerritory(List<Vector3> territoryArea, Vector3 position, Vector3 direction, float distance, out Vector3 normal)
	{
		m_lineStart1 = position;
		m_lineEnd1 = position + direction * distance;

		for (int i = 0, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (HitSegments(territoryArea[i], territoryArea[i + 1]))
			{
				normal = Vector3.Cross(
					(territoryArea[i + 1] - territoryArea[i]).normalized, Vector3.up).normalized;
				return true;
			}
		}

		if (HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0]))
		{
			normal = Vector3.Cross(
				(territoryArea[0] - territoryArea[territoryArea.Count - 1]).normalized, Vector3.up).normalized;
			return true;
		}

		normal = Vector3.zero;
		return false;
	}

	static bool HitCircleSegment(Vector3 lineStart0, Vector3 lineEnd0)
	{
		/*
		参考にしたサイト 
		https://yttm-work.jp/collision/collision_0006.html
		 */

		// ベクトルの作成
		Vector2 startToCenter = new Vector2(m_lineStart1ToVec2.x - lineStart0.x, m_lineStart1ToVec2.z - lineStart0.z);
		Vector2 endToCenter = new Vector2(m_lineStart1ToVec2.x - lineEnd0.x, m_lineStart1ToVec2.z - lineEnd0.z);
		Vector2 startToEnd = new Vector2(lineEnd0.x - lineStart0.x, lineEnd0.z - lineStart0.z);

		// 単位ベクトル化する
		Vector2 startToEndNormalized= startToEnd.normalized;

		/*
			射影した線分の長さ
				始点と円の中心で外積を行う
				※始点 => 終点のベクトルは単位化しておく
		*/
		// 線分と円の最短の長さが半径よりも小さい
		if (Mathf.Abs(startToCenter.x * startToEndNormalized.y 
			- startToEndNormalized.x * startToCenter.y) < m_radius)
		{
			// 始点 => 終点と始点 => 円の中心の内積を計算する
			float dot01 = startToCenter.x * startToEnd.x + startToCenter.y * startToEnd.y;
			// 始点 => 終点と終点 => 円の中心の内積を計算する
			float dot02 = endToCenter.x * startToEnd.x + endToCenter.y * startToEnd.y;

			// 二つの内積の掛け算結果が0以下なら当たり
			if (dot01 * dot02 <= 0.0f)
				return true;
			else
				return false;
		}
		else
			return false;	
	}

	static bool HitSegments(Vector3 lineStart0, Vector3 lineEnd0, out Vector3 hitPoint)
	{
		hitPoint = Vector3.zero;

		float d = (lineEnd0.x - lineStart0.x) * (m_lineEnd1.z - m_lineStart1.z) - (lineEnd0.z - lineStart0.z) * (m_lineEnd1.x - m_lineStart1.x);

		if (d == 0.0f)
			return false;

		float u = ((m_lineStart1.x - lineStart0.x) * (m_lineEnd1.z - m_lineStart1.z) - (m_lineStart1.z - lineStart0.z) * (m_lineEnd1.x - m_lineStart1.x)) / d;
		float v = ((m_lineStart1.x - lineStart0.x) * (lineEnd0.z - lineStart0.z) - (m_lineStart1.z - lineStart0.z) * (lineEnd0.x - lineStart0.x)) / d;

		if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
			return false;

		hitPoint.x = lineStart0.x + u * (lineEnd0.x - lineStart0.x);
		hitPoint.z = lineStart0.z + u * (lineEnd0.z - lineStart0.z);

		return true;
	}
	static bool HitSegments(Vector3 lineStart0, Vector3 lineEnd0)
	{
		float d = (lineEnd0.x - lineStart0.x) * (m_lineEnd1.z - m_lineStart1.z) - (lineEnd0.z - lineStart0.z) * (m_lineEnd1.x - m_lineStart1.x);

		if (d == 0.0f)
			return false;

		float u = ((m_lineStart1.x - lineStart0.x) * (m_lineEnd1.z - m_lineStart1.z) - (m_lineStart1.z - lineStart0.z) * (m_lineEnd1.x - m_lineStart1.x)) / d;
		float v = ((m_lineStart1.x - lineStart0.x) * (lineEnd0.z - lineStart0.z) - (m_lineStart1.z - lineStart0.z) * (lineEnd0.x - lineStart0.x)) / d;

		if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
			return false;

		return true;
	}
	static bool HitPointSegment(Vector3 linePoint0)
	{
		Vector2 lineStart1ToEnd1 = new Vector2(m_lineEnd1.x - m_lineStart1.x, m_lineEnd1.z - m_lineStart1.z);
		Vector2 linePoint0ToLineStart1 = new Vector2(linePoint0.x - m_lineStart1.x, linePoint0.z - m_lineStart1.z);

		float length0 = lineStart1ToEnd1.magnitude;
		float length1 = linePoint0ToLineStart1.magnitude;


		if (Mathf.Abs((lineStart1ToEnd1.x * linePoint0ToLineStart1.x 
			+ lineStart1ToEnd1.y * linePoint0ToLineStart1.y) - length0 * length1) < 0.0001f 
			&& length0 >= length1)
			return true;
		else
			return false;
	}
}
