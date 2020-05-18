using UnityEngine;

public class FenceCreate : MonoBehaviour
{
    [SerializeField]
    int stageWidth = 50;

    [SerializeField]
    int stageDepth = 50;

    [SerializeField]
    GameObject fence = null;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 vec = Vector3.zero;
        Quaternion qt = Quaternion.identity;

        for (int i = 0; i < stageWidth / 2; ++i)
        {
            vec = Vector3.zero;
            vec.x = (float)(i * 2 + 1);

            qt = Quaternion.identity;

            Instantiate(fence, vec, qt, transform);

            vec = Vector3.zero;
            vec.x = (float)((stageWidth - i * 2) - 1);
            vec.z = (float)stageDepth;

            qt = Quaternion.identity;
            qt.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            Instantiate(fence, vec, qt, transform);
        }
        for (int i = 0; i < stageDepth / 2; ++i)
        {
            vec = Vector3.zero;
            vec.z = (float)((stageDepth - i * 2) - 1);

            qt = Quaternion.identity;
            qt.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

            Instantiate(fence, vec, qt, transform);

            vec = Vector3.zero;
            vec.z = (float)(i * 2 + 1);
            vec.x = (float)stageWidth;

            qt = Quaternion.identity;
            qt.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
            Instantiate(fence, vec, qt, transform);
        }

    }


	void OnDrawGizmos()
	{
		UnityEngine.AI.NavMeshObstacle obstacle = fence.GetComponent<UnityEngine.AI.NavMeshObstacle>();

		Gizmos.color = Color.black;
		//left
		Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, stageDepth / 2), 
			new Vector3(obstacle.size.z, obstacle.size.y, obstacle.size.x * (stageDepth / 2)));
		//up
		Gizmos.DrawWireCube(new Vector3(stageWidth / 2, 0.0f, stageDepth),
			new Vector3(obstacle.size.x * (stageDepth / 2) + obstacle.size.x * 0.5f, obstacle.size.y, obstacle.size.z));
		//right
		Gizmos.DrawWireCube(new Vector3(stageWidth, 0.0f, stageDepth / 2),
			new Vector3(obstacle.size.z, obstacle.size.y, obstacle.size.x * (stageDepth / 2)));
		//down
		Gizmos.DrawWireCube(new Vector3(stageWidth / 2, 0.0f, 0.0f),
			new Vector3(obstacle.size.x * (stageDepth / 2) + obstacle.size.x * 0.5f, obstacle.size.y, obstacle.size.z));
	}
}
