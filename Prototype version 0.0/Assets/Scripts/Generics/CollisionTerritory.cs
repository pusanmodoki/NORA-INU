using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionTerritory
{
	public static bool HitLineTerritory(List<Vector3> territoryPoints, Vector3 position, float distance, float rightAdjust = 0.0f)
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
	

	static bool HitSegments(Vector3 lineStart0, Vector3 lineEnd0,
		Vector3 lineStart1, float rightDistance1, float rightAdjust1 = 0.0f)
	{
		lineStart0.x += rightAdjust1;

		float d = (lineEnd0.x - lineStart0.x) * (lineStart1.z - lineStart1.z) - (lineEnd0.z - lineStart0.z) * (lineStart1.x + rightDistance1 - lineStart1.x);

		if (d == 0.0f)
			return false;

		float u = ((lineStart1.x - lineStart0.x) * (lineStart1.z - lineStart1.z) - (lineStart1.z - lineStart0.z) * (lineStart1.x + rightDistance1 - lineStart1.x)) / d;
		float v = ((lineStart1.x - lineStart0.x) * (lineEnd0.z - lineStart0.z) - (lineStart1.z - lineStart0.z) * (lineEnd0.x - lineStart0.x)) / d;

		if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
			return false;

		return true;
	}
}
