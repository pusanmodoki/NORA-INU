using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMesh : MonoBehaviour
{
    [SerializeField]
    float m_radius = 1.0f;

    [SerializeField]
    int m_div = 30;

    [SerializeField]
    Material m_material = null;

    Mesh m_mesh = null;
    MeshRenderer m_renderer = null;



    // Start is called before the first frame update
    void Awake()
    {
        m_mesh = gameObject.AddComponent<MeshFilter>().mesh;
        m_renderer = gameObject.AddComponent<MeshRenderer>();

        m_renderer.material = m_material;

        List<Vector3> points = new List<Vector3>();
        List<int> triangles = new List<int>();

        points.Add(Vector3.zero);
        for(int i = 0; i < m_div * 2; ++i)
        {
            float rad = i * 180.0f / m_div * Mathf.Deg2Rad;
            Vector3 point = Vector3.zero;
            point.x = -Mathf.Cos(rad) * m_radius;
            point.z = Mathf.Sin(rad) * m_radius;
            points.Add(point);
        }
        for(int i = 0; i < points.Count - 2; ++i)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        triangles.Add(0);
        triangles.Add(points.Count - 1);
        triangles.Add(1);

        m_mesh.vertices = points.ToArray();
        m_mesh.triangles = triangles.ToArray();

    }
}
