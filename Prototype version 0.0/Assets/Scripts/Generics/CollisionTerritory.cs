using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionTerritory
{
	public static bool HitLineTerritory(List<Vector3> territoryPoints, Vector3 position, float distance)
	{
		Vector2 positionToVec2 = new Vector2(position.x, position.z);
		int hitCount = 0;

		for (int i = 0, count = territoryPoints.Count - 1; i < count; ++i)
		{
			if (HitSegments(territoryPoints[i], territoryPoints[i + 1], positionToVec2, distance))
				++hitCount;
		}

		return hitCount % 2 == 1;
	}

	public static bool HitCircleTerritory(List<Vector3> territoryPoints, Vector3 position, float radius)
	{
		Vector2 positionToVec2 = new Vector2(position.x, position.z);
		int hitCount = 0;

		for (int i = 0, count = territoryPoints.Count - 1; i < count; ++i)
		{
			if (HitCircleLine(territoryPoints[i], territoryPoints[i + 1], positionToVec2, radius))
				++hitCount;
		}

		return hitCount % 2 == 1;
	}

	// 2Dベクトルの外積
	static float Vector2Cross(Vector2 v1, Vector2 v2)
	{
		return v1.x * v2.y - v1.y * v2.x;
	}

	static bool HitSegments(Vector3 segmentStart0, 
		Vector3 segmentEnd0, Vector2 segment1, float distance)
	{
		Vector2[] toVector2s = {
				new Vector2(segmentStart0.x, segmentStart0.z),
				new Vector2(segmentEnd0.x, segmentEnd0.z) };
		Vector2 directionSegment0 = toVector2s[1] - toVector2s[0];
		Vector2 directionSegment1 = Vector2.right * distance;
		Vector2 directionSegments = directionSegment1 - directionSegment0;

		float crossDirections = Vector2Cross(directionSegment0, directionSegment1);

		if (Mathf.Epsilon > crossDirections && crossDirections < Mathf.Epsilon)
			return false;

		float crossSegment0 = Vector2Cross(directionSegments, directionSegment0);
		float crossSegment1 = Vector2Cross(directionSegments, directionSegment1);

		float t0 = crossSegment1 / crossDirections;
		float t1 = crossSegment0 / crossDirections;

		if (t0 + Mathf.Epsilon < 0.0f || t0 - Mathf.Epsilon > 1.0f
			|| t1 + Mathf.Epsilon < 0.0f || t1 - Mathf.Epsilon > 1.0f)
			return false;

		return true;
	}

	static bool HitCircleLine(Vector3 point0, Vector3 point1, Vector2 position, float radius)
	{
		Vector2 point0ToVec2 = new Vector2(point0.x, point0.z),
					point1ToVec2 = new Vector2(point1.x, point1.z);

		Vector2 segment = point1 - point0;
		Vector2 point0ToCircle = position - point0ToVec2;
		Vector2 point1ToCircle = position - point1ToVec2;

		float distance = Vector2.Scale(segment, point0ToCircle).magnitude / segment.magnitude;

		if (distance > radius) return false;

		if (Vector2.Dot(point0ToCircle, segment) * Vector2.Dot(point1ToCircle, segment) <= 0)
			return true;
		else if (radius > point0ToCircle.magnitude || radius > point1ToCircle.magnitude)
			return true;
		else
			return false;
	}
}
