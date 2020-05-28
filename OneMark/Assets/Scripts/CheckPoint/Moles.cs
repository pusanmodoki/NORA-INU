using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moles : BaseCheckPoint
{
    [SerializeField]
    Animator m_animation = null;

    [SerializeField]
    Animator m_boneAnimation = null;



    public override void LinkPlayer(int playerInstanceID)
    {
        base.LinkPlayer(playerInstanceID);
        Escape();
    }

    public override void UnlinkPlayer()
    {
        base.UnlinkPlayer();
        Appear();
    }

    private void Escape()
    {
        m_animation.SetTrigger("Escape");
    }

    private void Appear()
    {
        m_animation.SetTrigger("Appear");
        m_boneAnimation.SetTrigger("Confiscated");
    }

    public override void UpdatePoint()
    {
    }
}
