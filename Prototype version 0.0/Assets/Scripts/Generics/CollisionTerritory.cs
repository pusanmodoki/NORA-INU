using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionTerritory
{
	public static bool HitCircleTerritory(List<Vector3> territoryPoints, Vector3 position, float radius)
	{
		int hitCount = 0;

		for (int i = 0, count = territoryPoints.Count - 1; i < count; ++i)
		{
			if (HitCircleLine(territoryPoints[i], territoryPoints[i + 1], position, radius))
				++hitCount;
		}

		return hitCount % 2 == 1;
	}

	static bool HitCircleLine(Vector3 point0, Vector3 point1, Vector3 position, float radius)
	{
		Vector3 segment = point1 - point0;
		Vector3 point0ToCircle = position - point0;
		Vector3 point1ToCircle = position - point1;

		float distance = Vector3.Scale(segment, point0ToCircle).magnitude / segment.magnitude;

		if (distance > radius) return false;

		if (Vector3.Dot(point0ToCircle, segment) * Vector3.Dot(point1ToCircle, segment) <= 0)
			return true;
		else if (radius > point0ToCircle.magnitude || radius > point1ToCircle.magnitude)
			return true;
		else
			return false;
	}
}
