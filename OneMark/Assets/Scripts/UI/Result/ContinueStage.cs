using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueStage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

#if UNITY_EDITOR
		Debug.Log("KARIZISSO");
#endif
		//GetComponent<SceneTransButton>().m_sceneName = SceneTransManager.nowSceneName;
	}
}
