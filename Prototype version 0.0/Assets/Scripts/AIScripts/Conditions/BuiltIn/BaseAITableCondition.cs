//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy AI Components 
/// </summary>
namespace AIComponent
{
    /// <summary>
    /// Table実行条件を格納したBaseAITableCondition 
    /// </summary>
    public abstract class BaseAITableCondition : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>Condition Name (debug only)</summary>
        public string dConsitionName { get { return m_dConditionName; } }
        
        /// <summary>Condition Name (debug only)</summary>
        [SerializeField, Tooltip("Condition name (debug only)")]
        string m_dConditionName = "";
#endif
        /// <summary>
        /// [IsAITableCondition]
        /// return: テーブル条件を満たしているか否か
        /// </summary>
        public abstract bool IsAITableCondition();
    }
}