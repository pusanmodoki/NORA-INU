using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class StageSelectDataLoader : MonoBehaviour
{
	[System.Serializable]
	struct WorldInfos
	{
		public WorldInfos(GameObject worldObject, GameObject[] stageObjects)
		{
			m_worldObject = worldObject;
			m_stageObjects = stageObjects;
		}

		public GameObject worldObject { get { return m_worldObject; } }
		public GameObject[] stageObjects { get { return m_stageObjects; } }

		[SerializeField]
		GameObject m_worldObject;

		[SerializeField]
		GameObject[] m_stageObjects;
	}

	[SerializeField]
	WorldInfos[] m_worldInfos = null;
	
	void Awake()
	{
		int index = DataManager.instance.saveData.numClearStages;
		if (index == -1) index = 3;
		Vector2Int nextStage = OneMarkSceneManager.cStageSceneIndexes[index];

		for (int i = 0; i < m_worldInfos.Length; ++i)
		{
			if (nextStage.x <= i)
			{
				m_worldInfos[i].worldObject.SetActive(false);
				continue;
			}
			else
				m_worldInfos[i].worldObject.SetActive(true);

			for (int k = 0; k < m_worldInfos[i].stageObjects.Length; ++k)
			{
				if (nextStage.x - 1 ==  i && nextStage.y <= k)
					m_worldInfos[i].stageObjects[k].SetActive(false);
				else
					m_worldInfos[i].stageObjects[k].SetActive(true);
			}
		}
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
