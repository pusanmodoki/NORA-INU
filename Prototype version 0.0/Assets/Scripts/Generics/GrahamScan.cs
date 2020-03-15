using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GrahamScan
{
	struct SortComparer : IComparer<Vector3>
	{
		public SortComparer(Vector3 startPoint)
		{
			m_startPoint = startPoint;
		}

		int IComparer<Vector3>.Compare(Vector3 left, Vector3 right)
		{
			float result0 = (left.x - m_startPoint.x) / (left.z - m_startPoint.z);
			float result1 = (right.x - m_startPoint.x) / (right.z - m_startPoint.z);

			//return result0.CompareTo(result1);
			if(result0 > result1) return -1;
			else if (result0 < result1) return 1;
			else return 0;
		}

		Vector3 m_startPoint;
	}

	public static int Run(List<Vector3> points)
	{
		if (points.Count <= 2) return 0;

		int startPoint = FindStartPointIndex(points), iterator = 2;

		points.SwapIndex(startPoint, 0);

		//points.OrderBy(vec => vec, new SortComparer(points[0]));
		SortComparer sortComparer = new SortComparer(points[0]);
		points.Sort(1, points.Count - 1, new SortComparer(points[0]));

		for (int i = 3, count = points.Count; i < count; ++i)
		{
			while (Ccw(points, iterator - 1, iterator, i))
				iterator -= 1;

			iterator += 1;
			points.SwapIndex(iterator, i);
		}

		return iterator + 1;
	}

	static int FindStartPointIndex(List<Vector3> points)
	{
		int result = points.FindMinIndex((Vector3 left, Vector3 right) => left.z < right.z);

		for (int i = 0, count = points.Count; i < count; ++i)
		{
			if (Mathf.Abs(points[i].z - points[result].z) <= Mathf.Epsilon
				&& points[i].x > points[result].x)
			{
				result = i;
			}
		}
		
		return result;
	}

	static bool Ccw(List<Vector3> points, int index0, int index1, int index2)
	{
		return ((points[index1].x - points[index0].x) * (points[index2].z - points[index0].z) 
			- (points[index1].z - points[index0].z) * (points[index2].x - points[index0].x)) <= 0;
	}
}