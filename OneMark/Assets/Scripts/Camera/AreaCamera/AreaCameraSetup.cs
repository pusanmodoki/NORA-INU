using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCameraSetup : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        Vector3 pos = new Vector3(MainGameManager.instance.stageSize.x / 2.0f, 10.0f, MainGameManager.instance.stageSize.y / 2.0f);
        transform.position = pos;

        camera.orthographic = true;
        camera.orthographicSize = MainGameManager.instance.stageSize.y / 2.0f;
    }

}
