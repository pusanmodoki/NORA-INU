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
		out Vector3 normal, float circleRadius, float rayDistance = 1000.0f)
	{
		if (territoryArea.Count <= 1)
		{
			normal = Vector3.zero;
			return false;
		}

		Vector3 shortestHitPoint = position * (rayDistance * 2.0f), hitPoint;
		int hitCount = 0;
		bool isHit = false;

		m_lineStart1 = position;
		m_lineStart1ToVec2.Set(position.x, 0.0f, position.z);
		m_lineEnd1.Set(position.x, position.y, position.z + rayDistance);
		m_radius = circleRadius;

		for (int i = 0, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (HitCircleSegment(territoryArea[i], territoryArea[i + 1]))
			{
				isHit = true;
				break;
			}
			if (HitSegments(territoryArea[i], territoryArea[i + 1], out hitPoint))
			{
				if ((shortestHitPoint - position).sqrMagnitude > (hitPoint - position).sqrMagnitude)
					shortestHitPoint = hitPoint;
				++hitCount;
			}
		}

		if (!isHit)
		{
			if (HitCircleSegment(territoryArea[territoryArea.Count - 1], territoryArea[0]))
				isHit = true;
			if (!isHit && HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0]))
				++hitCount;
		}

		normal = shortestHitPoint;

		normal = Vector3.Cross(shortestHitPoint, Vector3.up);

		return isHit | hitCount % 2 == 1;
	}

	public static bool HitCircleTerritory(List<Vector3> territoryArea, Vector3 position, float radius, float rayDistance = 1000.0f)
	{
		if (territoryArea.Count <= 1) return false;

		int hitCount = 0;

		m_lineStart1 = position;
		m_lineStart1ToVec2.Set(position.x, 0.0f, position.z);
		m_lineEnd1.Set(position.x, position.y, position.z + rayDistance);
		m_radius = radius;

		for (int i = 0, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (HitCircleSegment(territoryArea[i], territoryArea[i + 1]))
				return true;
			if (HitSegments(territoryArea[i], territoryArea[i + 1]))
				++hitCount;
		}

		if (HitCircleSegment(territoryArea[territoryArea.Count - 1], territoryArea[0]))
			return true;
		if (HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0]))
			++hitCount;

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
				normal = Vector3.Cross((territoryArea[i + 1] - territoryArea[i]).normalized, Vector3.up);
				return true;
			}
		}

		if (HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0]))
		{
			normal = Vector3.Cross((territoryArea[territoryArea.Count - 1] 
				- territoryArea[0]).normalized, Vector3.up);
			return true;
		}

		normal = Vector3.zero;
		return false;
	}

	static bool HitCircleSegment(Vector3 lineStart0, Vector3 lineEnd0)
	{
		lineStart0.y = lineEnd0.y = 0.0f;
		Vector3 a = (m_lineStart1ToVec2 - lineStart0);
		Vector3 b = (m_lineStart1ToVec2 - lineEnd0);
		Vector3 s = (lineEnd0 - lineStart0);

		float d = Vector3.Scale(s, a).magnitude / s.magnitude;
		if (d > m_radius) return false;

		if ((a.x * s.x +  a.z * s.z) * (b.x * s.x + b.z * s.z) <= 0.0f)
			return true;
		else if (m_radius > a.magnitude || m_radius > b.magnitude)
			return true;
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
}
