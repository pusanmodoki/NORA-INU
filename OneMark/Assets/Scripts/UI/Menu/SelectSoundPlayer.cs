using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSoundPlayer : MonoBehaviour
{
    [SerializeField]
    AudioSource select = null;

    [SerializeField]
    AudioSource enter = null;

    [SerializeField]
    bool isThisPlayDetect = false;

    // Update is called once per frame
    void Update()
    {
        if (!isThisPlayDetect) return;
        if (Input.GetButtonDown("Horizontal") ||
            Input.GetButtonDown("Vertical"))
        {
            select.Play();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            enter.Play();
        }
    }

    public void SelectPlay()
    {
        select.Play();
    }

    public void EnterPlay()
    {
        enter.Play();
    }
}
