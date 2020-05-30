using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaBorderMesh : MonoBehaviour
{
	public MeshRenderer meshRenderer { get { return m_renderer; } }

	[SerializeField]
    float m_height = 1.0f;

    [SerializeField]
    private List<Vector3> m_meshPoints = new List<Vector3>();
    [SerializeField]
    private List<Vector2> m_uvs = new List<Vector2>();
    [SerializeField]
    private List<int> m_triangles = new List<int>();



    [SerializeField]
    Material m_material = null;

    GameObject m_meshObject = null;

    Mesh m_mesh = null;
    MeshRenderer m_renderer = null;
    
    // Start is called before the first frame update
    public void InitMesh()
    {
        m_meshObject = new GameObject("AreaBorderMesh");
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
			m_meshObject, OneMarkSceneManager.instance.nowScene);

		m_meshObject.AddComponent<MeshFilter>();
        m_renderer = m_meshObject.AddComponent<MeshRenderer>();
        m_mesh = m_meshObject.GetComponent<MeshFilter>().mesh;
        m_renderer.material = m_material;
    }

    public void CreateBorder(List<Vector3> territorialArea)
    {
        m_meshPoints.Clear();
        m_triangles.Clear();
        m_mesh.Clear();
        m_uvs.Clear();

        //m_meshPoints.Add(vec);
        m_meshPoints.AddRange(territorialArea);
        m_meshPoints.AddRange(territorialArea);

        for(int i = 0; i < territorialArea.Count; ++i)
        {
            Vector3 point = m_meshPoints[i];
            point.y += m_height;
            m_meshPoints[i] = point;
        }

        for(int i = 0; i < territorialArea.Count - 1; ++i)
        {
            m_triangles.Add(i);
            m_triangles.Add(i + 1);
            m_triangles.Add(i + territorialArea.Count);

            m_triangles.Add(i + territorialArea.Count);
            m_triangles.Add(i + 1 + territorialArea.Count);
            m_triangles.Add(i + 1);
        }

        m_triangles.Add(territorialArea.Count - 1);
        m_triangles.Add(0);
        m_triangles.Add(m_meshPoints.Count - 1);

        m_triangles.Add(m_meshPoints.Count - 1);
        m_triangles.Add(territorialArea.Count);
        m_triangles.Add(0);


        m_mesh.vertices = m_meshPoints.ToArray();
        m_mesh.triangles = m_triangles.ToArray();
        m_mesh.uv = m_uvs.ToArray();

        //render.material.color = Color.red;

        //　Boundsの再計算
        m_mesh.RecalculateBounds();
        //　NormalMapの再計算
        m_mesh.RecalculateNormals();

    }
}
