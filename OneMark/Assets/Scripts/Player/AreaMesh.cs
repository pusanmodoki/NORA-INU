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

    private Mesh mesh = null;
    private MeshRenderer render = null;

    [SerializeField]
    private Material material = null;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerAndTerritoryManager.instance.mainPlayer;
        gameObject.AddComponent<MeshFilter>();
        render= gameObject.AddComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        render.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        MeshCreate();
    }

    public void MeshCreate()
    {
        Vector3 vec = player.allTerritorys[0].transform.position;

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

        //　Boundsの再計算
        mesh.RecalculateBounds();
        //　NormalMapの再計算
        mesh.RecalculateNormals();
    }
}
