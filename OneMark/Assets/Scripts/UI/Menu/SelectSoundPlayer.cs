using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSoundPlayer : MonoBehaviour
{
    [SerializeField]
    AudioSource select = null;

    [SerializeField]
    AudioSource enter = null;



    public void SelectPlay()
    {
        select.Play();
    }

    public void EnterPlay()
    {
        enter.Play();
    }
}
