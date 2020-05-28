using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator logoAnime = null;

    public void LogoOn()
    {
        logoAnime.SetTrigger("OnFlg");
    }

    public void LogoOff()
    {
        logoAnime.SetTrigger("OffFlg");
    }

}
