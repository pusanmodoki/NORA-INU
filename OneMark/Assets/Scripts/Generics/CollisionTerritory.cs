using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionTerritory
{
	public static bool HitCircleTerritory(List<Vector3> territoryArea, Vector3 position, float rayDistance, float radius = 0.0f)
	{
		int hitCount = 0;

		for (int i = 0, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (HitSegments(territoryArea[i], territoryArea[i + 1], position, rayDistance))
				++hitCount;
		}

		if (HitSegments(territoryArea[territoryArea.Count - 1], territoryArea[0], position, rayDistance, radius))
			++hitCount;

		return hitCount % 2 == 1;
	}

	public static bool HitRayTerritory(List<Vector3> territoryArea, Vector3 position, Vector3 direction, float distance, out Vector3 normal)
	{
		for (int i = 0, count = territoryArea.Count - 1; i < count; ++i)
		{
			if (HitSegments(territoryArea[i], territoryArea[i + 1], position, position + direction * distance))
			{
				normal = Vector3.Cross((territoryArea[i + 1] - territoryArea[i]).normalized, Vector3.up);
				return true;
			}
		}

		if (HitSegments(territoryArea[territoryArea.Count - 1],
			territoryArea[0], position, position + direction * distance))
		{
			normal = Vector3.Cross((territoryArea[territoryArea.Count - 1] 
				- territoryArea[0]).normalized, Vector3.up);
			return true;
		}

		normal = Vector3.zero;
		return false;
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
	static bool HitSegments(Vector3 lineStart0, Vector3 lineEnd0, Vector3 lineStart1, Vector3 lineEnd1)
	{
		float d = (lineEnd0.x - lineStart0.x) * (lineStart1.z - lineStart1.z) - (lineEnd0.z - lineStart0.z) * (lineEnd1.x - lineStart1.x);

		if (d == 0.0f)
			return false;

		float u = ((lineStart1.x - lineStart0.x) * (lineStart1.z - lineStart1.z) - (lineStart1.z - lineStart0.z) * (lineEnd1.x - lineStart1.x)) / d;
		float v = ((lineStart1.x - lineStart0.x) * (lineEnd0.z - lineStart0.z) - (lineStart1.z - lineStart0.z) * (lineEnd0.x - lineStart0.x)) / d;

		if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
			return false;

		return true;
	}
}
