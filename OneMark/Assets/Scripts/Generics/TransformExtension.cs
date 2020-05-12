using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transformを拡張するIteratorExtension
/// </summary>
public static class TransformExtension 
{
	/// <summary>
	/// [LocalToWorldPosition]
	/// Scaleの影響を受けないLocal->World座標変換
	/// return: World position
	/// 引数1: <this>
	/// 引数1: local position
	/// </summary>
	public static Vector3 LocalToWorldPosition(this Transform self, Vector3 localPosition)
	{
		Matrix4x4 matrix = Matrix4x4.identity;

		matrix *= Matrix4x4.Translate(self.position);
		matrix *= Matrix4x4.Rotate(self.rotation);

		return matrix.MultiplyPoint(localPosition);
	}
}
