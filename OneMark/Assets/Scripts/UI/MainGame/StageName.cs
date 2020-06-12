using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageName : MonoBehaviour
{
	[SerializeField]
	Sprite[] m_numberSprites = null;
	[SerializeField]
	Image[] m_defaultUIChildrens = new Image[3];
	[SerializeField]
	Image[] m_tutorialUIChildrens = new Image[2];

    // Start is called before the first frame update
    void Awake()
    {
		var index = OneMarkSceneManager.instance.nowStageSceneIndex;

		if (index == OneMarkSceneManager.cInvalidStageIndex)
			return;

		if (index.x != 0)
		{
			foreach (var e in m_defaultUIChildrens)
				e.enabled = true;
			foreach (var e in m_tutorialUIChildrens)
				e.enabled = false;

			m_defaultUIChildrens[0].sprite = m_numberSprites[index.x - 1];
			m_defaultUIChildrens[2].sprite = m_numberSprites[index.y - 1];
		}
		else
		{
			foreach (var e in m_defaultUIChildrens)
				e.enabled = false;
			foreach (var e in m_tutorialUIChildrens)
				e.enabled = true;

			m_tutorialUIChildrens[1].sprite = m_numberSprites[index.y - 1];
		}
    }
}
