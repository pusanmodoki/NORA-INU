using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerEvent : MonoBehaviour
{
	public static readonly string cDefaultEnable = "Enable";
	public static readonly string cDefaultDisable = "Disable";

	public abstract void OnTrigger(string key);
}
