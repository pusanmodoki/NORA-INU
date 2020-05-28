using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField]
    private StageSlide stageSlide = null;

    // ワールドナンバー取ってくる
    public int worldNum { get { return instance.stageSlide.worldIndex; } }

    // 自分自身
    static public StageSelectManager instance { get; private set; }

    void Awake()
    {
        instance = this;
    }
}
