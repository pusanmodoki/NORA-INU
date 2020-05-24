using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMesh : MonoBehaviour
{
    private PlayerAndTerritoryManager.PlayerInfo player = null;

    [SerializeField]
    private List<Vector3> meshPoints = new List<Vector3>();
    [SerializeField]
    private List<Vector2> uvs = new List<Vector2>();
    [SerializeField]
    private List<int> triangles = new List<int>();

    [SerializeField]
    private List<Vector3> safetyMeshPoints = new List<Vector3>();
    [SerializeField]
    private List<Vector2> safetyUvs = new List<Vector2>();
    [SerializeField]
    private List<int> safetyTriangles = new List<int>();


    private Mesh mesh = null;
    private MeshRenderer render = null;

    private Mesh safetyMesh = null;
    private MeshRenderer safetyRender = null;

    [SerializeField]
    private Material m_material = null;
    [SerializeField]
    private Material m_safetyMaterial = null;

    private GameObject m_meshObject = null;
    private GameObject m_safetyMeshObject = null;


    // Start is called before the first frame update
    void Start()
    {
        player = PlayerAndTerritoryManager.instance.mainPlayer;

        m_meshObject = new GameObject("AreaMesh");

        m_meshObject.AddComponent<MeshFilter>();
        render= m_meshObject.AddComponent<MeshRenderer>();
        mesh = m_meshObject.GetComponent<MeshFilter>().mesh;
        render.material = m_material;
        m_meshObject.layer = LayerMask.NameToLayer("Area");

        m_safetyMeshObject = new GameObject("SafeAreaMesh");

        m_safetyMeshObject.AddComponent<MeshFilter>();
        safetyRender = m_safetyMeshObject.AddComponent<MeshRenderer>();
        safetyMesh = m_safetyMeshObject.GetComponent<MeshFilter>().mesh;
        safetyRender.material = m_safetyMaterial;
        m_safetyMeshObject.layer = LayerMask.NameToLayer("Area");
    }

    // Update is called once per frame
    void Update()
    {
        MeshCreate();
        SafetyMeshCreate();
    }

    public void MeshCreate()
    {
        if (player.territorialArea.Count < 3) { return; }
        Vector3 vec = player.territorialArea[0];

        // 初期化
        meshPoints.Clear();
        triangles.Clear();
        mesh.Clear();
        uvs.Clear();

        meshPoints.Add(vec);
        meshPoints.AddRange(player.territorialArea);

        for(int i = 0; i < meshPoints.Count; ++i)
        {
            Vector3 pos = meshPoints[i];
            pos.y = 0.0f;
            meshPoints[i] = pos;
            uvs.Add(Vector2.zero);
        }

        for(int i = 0; i < meshPoints.Count - 2; ++i)
        {
            triangles.Add(0);
            triangles.Add(meshPoints.Count - 1 - i);
            triangles.Add(meshPoints.Count - 1 - i - 1);
        }

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(meshPoints.Count - 1);

        mesh.vertices = meshPoints.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        //render.material.color = Color.red;

        //　Boundsの再計算
        mesh.RecalculateBounds();
        //　NormalMapの再計算
        mesh.RecalculateNormals();
    }

    public void SafetyMeshCreate()
    {
        if(player.safetyTerritorialArea.Count < 3) { return; }
        Vector3 vec = player.safetyTerritorialArea[0];

        // 初期化
        safetyMeshPoints.Clear();
        safetyTriangles.Clear();
        safetyMesh.Clear();
        safetyUvs.Clear();

        safetyMeshPoints.Add(vec);
        safetyMeshPoints.AddRange(player.safetyTerritorialArea);

        for (int i = 0; i < safetyMeshPoints.Count; ++i)
        {
            Vector3 pos = safetyMeshPoints[i];
            pos.y = 1.0f;
            safetyMeshPoints[i] = pos;
            safetyUvs.Add(Vector2.zero);
        }

        for (int i = 0; i < safetyMeshPoints.Count - 2; ++i)
        {
            safetyTriangles.Add(0);
            safetyTriangles.Add(safetyMeshPoints.Count - 1 - i);
            safetyTriangles.Add(safetyMeshPoints.Count - 1 - i - 1);
        }

        safetyTriangles.Add(0);
        safetyTriangles.Add(1);
        safetyTriangles.Add(safetyMeshPoints.Count - 1);

        safetyMesh.vertices = safetyMeshPoints.ToArray();
        safetyMesh.triangles = safetyTriangles.ToArray();
        safetyMesh.uv = safetyUvs.ToArray();


        //　Boundsの再計算
        safetyMesh.RecalculateBounds();
        //　NormalMapの再計算
        safetyMesh.RecalculateNormals();
    }

}
