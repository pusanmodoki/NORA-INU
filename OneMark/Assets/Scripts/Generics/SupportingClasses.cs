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
