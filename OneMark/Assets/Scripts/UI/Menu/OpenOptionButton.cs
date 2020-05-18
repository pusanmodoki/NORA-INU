using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenOptionButton : MonoBehaviour
{
	public void OnButton()
	{
		OneMarkSceneManager.instance.SetActiveOptionScene(!OneMarkSceneManager.instance.isActiveOption);
	}
}
