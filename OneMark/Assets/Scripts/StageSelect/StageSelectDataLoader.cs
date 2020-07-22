using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class StageSelectDataLoader : MonoBehaviour
{
	[SerializeField]
	GameObject[] m_worldObjects = null;
	[SerializeField]
	GameObject[] m_stageObjects = null;

	void Awake()
	{
		Vector2Int nextStage = OneMarkSceneManager.cStageSceneIndexes[DataManager.instance.saveData.numClearStages - 1];

		for (int i = 0; i < m_worldObjects.Length; ++i)
		{
			if (nextStage.x <= i) m_worldObjects[i].SetActive(false);
			else m_worldObjects[i].SetActive(true);
		}

		for (int i = 0; i < m_stageObjects.Length; ++i)
			if (nextStage.x <= i) m_worldObjects[i].SetActive(false);
			else m_worldObjects[i].SetActive(true);
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
