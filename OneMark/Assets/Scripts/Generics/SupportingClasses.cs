using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RendererMaterial
{
	public RendererMaterial(SkinnedMeshRenderer meshRenderer,
		int materialIndex)
	{
		m_meshRenderer = meshRenderer;
		m_materialIndex = materialIndex;
	}

	public SkinnedMeshRenderer meshRenderer { get { return m_meshRenderer; } }
	public Material material { get { return m_meshRenderer.materials[materialIndex]; } }
	public int materialIndex { get { return m_materialIndex; } }

	[SerializeField]
	SkinnedMeshRenderer m_meshRenderer;
	[SerializeField]
	int m_materialIndex;
}

[System.Serializable]
public struct BoxCastInfos
{
	public BoxCastInfos(LayerMaskEx layerMask, Vector3 center, Vector3 size)
	{
		m_layerMask = layerMask;
		m_center = center;
		m_size = size;
	}

	public LayerMaskEx layerMask { get { return m_layerMask; } }
	public Vector3 center { get { return m_center; } }
	public Vector3 size { get { return m_size; } }
	public Vector3 overlapSize { get { return m_size * 0.5f; } }

	[SerializeField]
	LayerMaskEx m_layerMask;
	[SerializeField]
	Vector3 m_center;
	[SerializeField]
	Vector3 m_size;

	public Vector3 WorldCenter(Transform transform) { return transform.LocalToWorldPosition(m_center); }

	public bool BoxCast(Transform transform, Vector3 direction, float distance)
	{
		return Physics.BoxCast(WorldCenter(transform), overlapSize, direction,
			transform.rotation, distance, layerMask);
	}
	public bool BoxCast(Transform transform, Vector3 direction, out RaycastHit raycastHit, float distance)
	{
		return Physics.BoxCast(WorldCenter(transform), overlapSize, direction,
			out raycastHit, transform.rotation, distance, layerMask);
	}
	public RaycastHit[] BoxCastAll(Transform transform, Vector3 direction, float distance)
	{
		return Physics.BoxCastAll(WorldCenter(transform), overlapSize, direction,
			transform.rotation, distance, layerMask);
	}

	//debug only
#if UNITY_EDITOR
	public void DOnDrawGizmos(Transform transform, Color color, Vector3 direction, float distance)
	{
		Vector3 worldCenter = WorldCenter(transform);

		//Color
		Gizmos.color = color;
		//Ray
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.DrawRay(worldCenter, direction * distance);
		//Matrix
		Gizmos.matrix = Matrix4x4.Translate(worldCenter + direction * distance);
		Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
		//Draw Cube
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
	public void DCubeOnDrawGizmos(Transform transform, Color color)
	{
		//Color
		Gizmos.color = color;
		//Matrix
		Gizmos.matrix = Matrix4x4.Translate(WorldCenter(transform));
		Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
		//Draw Cube
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
#endif
}

[System.Serializable]
public struct AdvanceBoxCastInfos
{
	public AdvanceBoxCastInfos(LayerMaskEx layerMask, Vector3 center,
		Vector3 rotation, Vector3 size, float distance)
	{
		m_layerMask = layerMask;
		m_center = center;
		m_rotation = rotation;
		m_size = size;
		m_distance = distance;
	}

	public LayerMaskEx layerMask { get { return m_layerMask; } }
	public Quaternion rotation { get { return Quaternion.Euler(m_rotation); } }
	public Vector3 center { get { return m_center; } }
	public Vector3 size { get { return m_size; } }
	public Vector3 overlapSize { get { return m_size * 0.5f; } }
	float distance { get { return m_distance; } }

	[SerializeField]
	LayerMaskEx m_layerMask;
	[SerializeField]
	Vector3 m_center;
	[SerializeField]
	Vector3 m_rotation;
	[SerializeField]
	Vector3 m_size;
	[SerializeField]
	float m_distance;

	public Vector3 WorldCenter(Transform transform) { return transform.LocalToWorldPosition(m_center); }
	public Quaternion WorldRotation(Transform transform) { return transform.rotation * rotation; }

	public bool BoxCast(Transform transform, Vector3 direction)
	{
		return Physics.BoxCast(WorldCenter(transform), overlapSize, direction,
			WorldRotation(transform), distance, layerMask);
	}
	public bool BoxCast(Transform transform, Vector3 direction, out RaycastHit raycastHit)
	{
		return Physics.BoxCast(WorldCenter(transform), overlapSize, direction,
			out raycastHit, WorldRotation(transform), distance, layerMask);
	}
	public RaycastHit[] BoxCastAll(Transform transform, Vector3 direction)
	{
		return Physics.BoxCastAll(WorldCenter(transform), overlapSize, direction,
			WorldRotation(transform), distance, layerMask);
	}

	//debug only
#if UNITY_EDITOR
	public void DOnDrawGizmos(Transform transform, Color color, Vector3 direction)
	{
		Vector3 worldCenter = WorldCenter(transform);
		//Color
		Gizmos.color = color;
		//Ray
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.DrawRay(worldCenter, direction * distance);
		//Matrix
		Gizmos.matrix = Matrix4x4.Translate(worldCenter + direction * distance);
		Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
		//Draw Cube
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
	public void DOnDrawGizmos(Transform transform, Color color, Vector3 direction, float hitDistance)
	{
		Vector3 worldCenter = WorldCenter(transform);
		//Color
		Gizmos.color = color;
		//Ray
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.DrawRay(worldCenter, direction * hitDistance);
		//Matrix
		Gizmos.matrix = Matrix4x4.Translate(worldCenter + direction * hitDistance);
		Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
		//Draw Cube
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
	public void DCubeOnDrawGizmos(Transform transform, Color color)
	{
		//Color
		Gizmos.color = color;
		//Matrix
		Gizmos.matrix = Matrix4x4.Translate(WorldCenter(transform));
		Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
		//Draw Cube
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
#endif
}