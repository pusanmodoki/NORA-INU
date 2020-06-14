using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDisableMenu : MonoBehaviour
{
	[SerializeField]
	MenuInput m_menuInput = null;
	[SerializeField]
	Vector3Int[] m_disableStageAndIndex = null;

    // Start is called before the first frame update
    void Awake()
    {
		for(int i = 0; i < m_disableStageAndIndex.Length; ++i)
		{
			for (int k = i + 1; k < m_disableStageAndIndex.Length; ++k)
			{
				if (m_disableStageAndIndex[i].z > m_disableStageAndIndex[k].z)
				{
					var temp = m_disableStageAndIndex[k];
					m_disableStageAndIndex[k] = m_disableStageAndIndex[i];
					m_disableStageAndIndex[i] = temp;
				}
			}
		}

		int disableCounter = 0;
        foreach(var e in m_disableStageAndIndex)
		{
			Vector2Int stageIndex = new Vector2Int(e.x, e.y);

			if (stageIndex == OneMarkSceneManager.instance.nowStageSceneIndex)
				m_menuInput.RemoveMenu(e.z - disableCounter++);
		}
    }
}
