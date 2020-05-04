using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageName : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Text text = GetComponent<Text>();
        text.text = SceneTransManager.nowSceneName;
    }

}
