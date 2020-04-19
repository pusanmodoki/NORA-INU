using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceCreate : MonoBehaviour
{
    [SerializeField]
    int stageWidth = 50;

    [SerializeField]
    int stageHeight = 50;

    [SerializeField]
    GameObject fenceEnd = null;

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
            vec.z = (float)stageHeight;

            qt = Quaternion.identity;
            qt.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            Instantiate(fence, vec, qt, transform);
        }
        for (int i = 0; i < stageHeight / 2; ++i)
        {
            vec = Vector3.zero;
            vec.z = (float)((stageHeight - i * 2) - 1);

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
}
