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
	/// 関数を格納するAIFunctionContainer
	/// </summary>
	[System.Serializable]
	public class AIFunctionContainer
	{
		/// <summary> AI function </summary>
		[Tooltip("AI function")]
		public BaseAIFunction function = null;

#if UNITY_EDITOR
		/// <summary>Function name (debug only)</summary>
		[SerializeField, Tooltip("Function name (debug only)")]
		public string dFunctionName = "";

		/// <summary>
		/// [DNameSet]
		/// Set names
		/// </summary>
		public void DNameSet()
		{
			if (function == null) return;

			dFunctionName = function.functionName;
		}
#endif
	}


	/// <summary>
	/// AIの行動テーブルを格納するAITable class
	/// </summary>
	[System.Serializable]
    public class AITable
    {
        /// <summary>
        /// AITableの一要素を格納するTableElement struct
        /// </summary>
        [System.Serializable]
        public struct TableElement
		{
#if UNITY_EDITOR
			/// <summary>[constructor]</summary>
			TableElement(BaseAIFunction function, string dFunctionName, float probability)
			{
				this.function = function;
				this.probability = probability;
				this.dFunctionName = dFunctionName;
			}
#else
			/// <summary>[constructor]</summary>
			TableElement(BaseAIFunction function, float probability)
            {
                this.function = function;
                this.probability = probability;
            }
#endif

			/// <summary> AI function </summary>
			[Tooltip("AI function")]
            public BaseAIFunction function;
#if UNITY_EDITOR
			/// <summary>Function name (debug only)</summary>
			[SerializeField, Tooltip("Function name (debug only)")]
			public string dFunctionName;
#endif
			/// <summary>Selection probability (0.0f ~ 1.0f)</summary>
			[Tooltip("Selection probability (0.0f ~ 1.0f)"), Range(0.0f, 1.0f)]
            public float probability;
        }

        /// <summary>Condition名</summary>
        public BaseAICondition condition { get { return m_condition; } }
        /// <summary>テーブル名</summary>
        public string tableName { get { return m_tableName; } }
        /// <summary>有効か否か</summary>
        public bool isEnabledSelf { get { return m_isEnabled; } }
		/// <summary>常にtrueとするか？</summary>
		public bool isConditionAlwaysTrueSelf { get { return m_isConditionAlwaysTrue & !m_isConditionAlwaysFalse; } }
		/// <summary>常にfalseとするか？</summary>
		public bool isConditionAlwaysFalseSelf { get { return !m_isConditionAlwaysTrue & m_isConditionAlwaysFalse; } }
		/// <summary>条件反転フラグ</summary>
		public bool isConditionReversalSelf { get { return m_isConditionReversal; } }
		/// <summary>Table実行条件を満たしているか否か</summary>
		public bool isPossibleUpdate
		{
			get
			{
				if (m_isConditionAlwaysTrue | m_isConditionAlwaysFalse)
					return (m_isConditionAlwaysTrue & !m_isConditionAlwaysFalse) & m_isEnabled;
				else
					return (m_condition.IsCondition() ^ m_isConditionReversal) & m_isEnabled;
			}
		}
#if UNITY_EDITOR
		/// <summary>functions</summary>
		public TableElement[] dFunctions { get { return m_elements; } }
#endif

		/// <summary>Table Name</summary>
		[SerializeField, Tooltip("Table name")]
        string m_tableName = "AI table";
        /// <summary>Condition to Selection Table</summary>
        [SerializeField, Tooltip("Condition to selection table")]
        BaseAICondition m_condition = null;
#if UNITY_EDITOR
		/// <summary>Reload Flag (debug only)</summary>
		[SerializeField, Tooltip("Condition name (debug only)")]
		string m_dConditionName = "";
#endif
		/// <summary>Table is enabled?</summary>
		[SerializeField, Tooltip("Table is enabled?")]
        bool m_isEnabled = true;
		/// <summary>Condition result is always true?</summary>
		[SerializeField, Tooltip("Condition result is always true?")]
		bool m_isConditionAlwaysTrue = false;
		/// <summary>Condition result is always false?</summary>
		[SerializeField, Tooltip("Condition result is always false?")]
		bool m_isConditionAlwaysFalse = false;
		/// <summary>Condition result is Reversal?</summary>
		[SerializeField, Tooltip("Condition result is Reversal?")]
		bool m_isConditionReversal = false;
#if UNITY_EDITOR
		/// <summary>Reload Flag (debug only)</summary>
		[SerializeField, Tooltip("Reload flag (debug only)")]
        bool m_dIsReload = false;
#endif
        /// <summary>Table Elements</summary>
        [SerializeField, Header("Table functions"), Tooltip("Table elements")]
        TableElement[] m_elements = null;

        /// <summary>Agent</summary>
        AIAgent m_agent = null;
        /// <summary>確率テーブル</summary>
        float[] m_probabilityTable = null;

#if UNITY_EDITOR
		/// <summary>
		/// [DNameSet]
		/// Set names
		/// </summary>
		public void DNameSet()
		{
			if (m_condition != null) m_dConditionName = m_condition.dConditionName;
			for(int i = 0; i < m_elements.Length; i++)
			{
				if (m_elements[i].function != null)
					m_elements[i].dFunctionName = m_elements[i].function.functionName;
			}
		}
#endif

		/// <summary>
		/// [Start]
		/// 初期化する
		/// 引数1: this.AIAgent
		/// </summary>
		public void Start(AIAgent agent)
        {
            //代入
            m_agent = agent;

            //確率テーブル用変数
            float temp = 0.0f;
            //確率テーブル生成
            m_probabilityTable = new float[m_elements.Length];

            //要素ループ
            for (int i = 0; i < m_elements.Length; ++i)
            {
                //テーブルの
                m_probabilityTable[i] = temp + m_elements[i].probability;
                temp += m_elements[i].probability;
            }

            foreach (TableElement element in m_elements)
                if (element.function != null) element.function.StartAIFunction(agent, this);
        }

		/// <summary>
		/// [SelectionFunction]
		/// return: 確率で選出された実行関数
		/// </summary>
		public BaseAIFunction SelectionFunction()
		{
			//debug only
#if UNITY_EDITOR
			//Reload
			if (m_dIsReload)
			{
				m_dIsReload = false;
				Start(m_agent);
			}
#endif
			//Random
			float random = Random.value;

			//実行できる条件になった場合その関数クラスを返却
			for (int i = 0; i < m_elements.Length; ++i)
			{
				if (random <= m_probabilityTable[i])
					return m_elements[i].function;
			}

			//失敗
			return null;
		}

		/// <summary>
		/// [FindFunction]
		/// return: functionNameの要素があればBaseAIFunction, なければnull
		/// 引数1: BaseAIFunction->functionName
		/// </summary>
		public BaseAIFunction FindFunction(string functionName)
		{
			foreach (TableElement element in m_elements)
			{
				if (element.function.functionName == functionName)
					return element.function;
			}
			return null;
		}

        /// <summary>
        /// [SetEnabled]
        /// Enable情報のセット
        /// 引数1: セットする値
        /// </summary>
        public void SetEnabled(bool isEnabled)
        {
            m_isEnabled = isEnabled;
        }
	}
}