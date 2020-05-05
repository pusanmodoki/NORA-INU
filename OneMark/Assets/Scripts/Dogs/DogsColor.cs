using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogsColor : MonoBehaviour
{
    [SerializeField]
    GameObject body = null;

    [SerializeField]
    DogAIAgent dogInfo = null;
    private void Update()
    {
        dogInfo = GetComponent<DogAIAgent>();
        Material mat = body.GetComponent<SkinnedMeshRenderer>().materials[0];

        switch (dogInfo.aiAgentInstanceID)
        {
            case 0:
                {
                    mat.color = Color.blue;
                    break;
                }
            case 1:
                {
                    mat.color = Color.red;
                    break;
                }
            case 2:
                {
                    mat.color = Color.green;
                    break;
                }
        }
    }

}
