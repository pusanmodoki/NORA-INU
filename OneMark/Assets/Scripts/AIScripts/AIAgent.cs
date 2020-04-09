//作成者 : 植村将太
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy AI Components 
/// </summary>
namespace AIComponent
{
	/// <summary>
	/// AIを管理するAIAgent class
	/// </summary>
	public class AIAgent : MonoBehaviour
	{
		/// <summary> Tables get only property </summary>
		public AITable[] aITables { get { return m_aiTables; } }
		/// <summary> Not table Member functions get only property</summary>
		public AIFunctionContainer[] notTableMembers { get { return m_notTableMembers; } }
		/// <summary> NavMeshAgent get only property </summary>
		public NavMeshAgent navMeshAgent { get { return m_navMeshAgent; } }
		/// <summary> RigidBody get only property </summary>
		public Rigidbody rigidBody { get { return m_rigidbody; } }
		/// <summary> Now function </summary>
		public BaseAIFunction nowFunction { get; private set; } = null;
		/// <summary> Original instanceID</summary>
		public int aiAgentInstanceID { get; private set; } = -1;
		/// <summary> Prefab instanceID</summary>
		public int prefabInstanceID { get; private set; } = -1;
		/// <summary> NavMeshAgent -> updatePosition get and set property </summary>
		public bool navMeshUpdatePosition { get { return m_navMeshAgent.updatePosition; } set { m_navMeshAgent.updatePosition = value; } }
		/// <summary> NavMeshAgent -> updateRotation get and set property </summary>
		public bool navMeshUpdateRotation { get { return m_navMeshAgent.updateRotation; } set { m_navMeshAgent.updateRotation = value; } }

#if UNITY_EDITOR
		/// <summary> original instanceID (debug only) </summary>
		[SerializeField, Header("Debug Only"), Tooltip("Drawing original instanceID")]
		int m_dOriginalInstanceID = -1;
		/// <summary> Running function table name (debug only) </summary>
		[SerializeField, Tooltip("Running function table name (debug only)")]
		string m_dNowTable = "";
		/// <summary> Running table condition name (debug only) </summary>
		[SerializeField, Tooltip("Running table condition name (debug only)")]
		string m_dNowTableCondition = "";
		/// <summary> Running function name (debug only) </summary>
		[SerializeField, Tooltip("Running function name (debug only)")]
		string m_dNowFunction = "";

		/// <summary> Running Function Name (debug only) </summary>
		public string dNowTable { get { return m_dNowTable; } }
		/// <summary> Running Function Condition Name (debug only) </summary>
		public string dNowTableCondition { get { return m_dNowTableCondition; } }
		/// <summary> Running Function Name (debug only) </summary>
		public string dNowFunction { get { return m_dNowFunction; } }
#endif

		/// <summary> This nav mesh agent </summary>
		[SerializeField, Header("Reference"), Tooltip("This nav mesh agent")]
		NavMeshAgent m_navMeshAgent = null;
		/// <summary> This rigid body </summary>
		[SerializeField, Tooltip("This rigid body")]
		Rigidbody m_rigidbody = null;
		
		/// <summary> Startup ai table </summary>
		[SerializeField, Header("Table And Functions"), Tooltip("Startup ai table")]
		int m_beginTableIndex = 0;
		/// <summary> AITable list </summary>
		[SerializeField, Tooltip("AI table list")]
		AITable[] m_aiTables = null;
		/// <summary> AITable list </summary>
		[SerializeField, Tooltip("Not table member functions"), Space]
		AIFunctionContainer[] m_notTableMembers = null;

		/// <summary> Instancecounter </summary>
		static int m_instanceConter = 0;

		/// <summary> ひとつ前の実行設定関数クラス </summary>
		BaseAIFunction m_oldFunction = null;
		/// <summary> Function->End用 (敢えて少し実行しにくいようにしている) </summary>
		BaseAIFunction.UpdateIdentifier m_updateIdentifier = new BaseAIFunction.UpdateIdentifier();
		
		/// <summary> Begin Callback実行したか否か </summary>
		bool m_isBeginCallBackCompleted = false;

		/// <summary>
		/// [AllocateFunction]
		/// 新たに実行関数クラスを割り当てる
		/// </summary>
		public void AllocateFunction()
		{
			//old保存, 初期化
			BaseAIFunction oldSave = nowFunction;
			nowFunction = null;

			//検索ループ
			foreach (AITable table in m_aiTables)
			{
				//実行条件を満たしている
				if (table.isPossibleUpdate)
				{
					//確率を基に実行関数設定
					nowFunction = table.SelectionFunction();
				
					//Errorチェック
#if UNITY_EDITOR
					if (nowFunction == null)
					{
						Debug.LogError("Error!! AIAgent->FixedUpdate now function == null");
						nowFunction = oldSave;
						return;
					}
#endif
					//成功すれば終了側Callback呼び出し, フラグ初期化
					if (nowFunction != null)
					{
						//debug only, 関数表示用文字列初期化
#if UNITY_EDITOR
						m_dNowTable = table.tableName;
						m_dNowFunction = nowFunction.functionName;

						if (table.condition != null)
							m_dNowTableCondition = table.condition.dConditionName;
						else if (table.isConditionAlwaysTrueSelf)
							m_dNowTableCondition = "Always true";
						else if (table.isConditionAlwaysFalseSelf)
							m_dNowTableCondition = "Always false";
						else
							m_dNowTableCondition = "Not condition";
#endif

						m_oldFunction = oldSave;
						m_oldFunction?.AIEnd(nowFunction);
						m_isBeginCallBackCompleted = false;  //Beginをまだ呼び出していない
						break;
					}
				}
			}

			//念の為null check, nullなら戻す
			if (nowFunction== null)
			{
#if UNITY_EDITOR
				Debug.LogError("Error!! AIAgent->AllocateFunction not found function");
#endif
				nowFunction = oldSave;
			}
		}

		/// <summary>
		/// [ForceSpecifyFunction]
		/// 強制的に実行中の関数クラス停止させ、指定したものを実行させる
		/// 引数1: 実行させるfunction
		/// </summary>
		public void ForceSpecifyFunction(BaseAIFunction function)
		{
			//無効？
			if (function == null) return;

			//old保存, 初期化
			m_oldFunction = nowFunction;
			nowFunction = function;
			//Beginをまだ呼び出していない
			m_isBeginCallBackCompleted = false;
			//End Callback
			m_oldFunction?.AIEnd(nowFunction);

			//debug only, 関数表示用文字列初期化
#if UNITY_EDITOR
			m_dNowTable = function.tableName;
			m_dNowFunction = function.functionName;

			if (function.aiTable != null)
			{
				if (function.aiTable.condition != null)
					m_dNowTableCondition = function.aiTable.condition.dConditionName;
				else if (function.aiTable.isConditionAlwaysTrueSelf)
					m_dNowTableCondition = "Always true";
				else if (function.aiTable.isConditionAlwaysFalseSelf)
					m_dNowTableCondition = "Always false";
				else
					m_dNowTableCondition = "Not condition";
			}
			else
				m_dNowTableCondition = "Not condition";
#endif
		}

		/// <summary>
		/// [SetRigitFreezeAll]
		/// rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		/// </summary>
		public void SetRigitFreezeAll()
		{
			m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		/// <summary>
		/// [SetRigitFreezeExcludingPositionXZ]
		/// m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
		/// </summary>
		public void SetRigitFreezeExcludingPositionXZ()
		{
			m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
		}

		/// <summary>
		/// [SimulationNextTable]
		/// return: 現在フレームで実行されると判断されるAITable
		/// </summary>
		public AITable SimulationNextTable()
		{
			//検索ループ
			foreach (AITable table in m_aiTables)
			{
				//実行条件を満たしている
				if (table.isPossibleUpdate)
					return table;
			}

			return null;
		}
		/// <summary>
		/// [FindTable]
		/// return: tableNameの要素があればAITable, なければnull
		/// </summary>
		public AITable FindTable(string tableName)
		{
			foreach (AITable table in m_aiTables)
			{
				if (table.tableName == tableName)
					return table;
			}
			return null;
		}
		/// <summary>
		/// [FindFunction]
		/// return: tableNameの要素があり, その中にfunctionNameの要素があればBaseAIFunction, なければnull
		/// </summary>
		public BaseAIFunction FindFunction(string tableName, string functionName)
		{
			foreach (AITable table in m_aiTables)
			{
				if (table.tableName == tableName)
				{
					BaseAIFunction result = table.FindFunction(functionName);
					if (result != null) return result;
				}
			}
			return null;
		}

		/// <summary> [Awake] </summary>
		protected void Awake()
		{
			if (prefabInstanceID == -1)
				prefabInstanceID = GetInstanceID();
		}
		/// <summary> [Start] </summary>
		protected void Start()
		{
			//Set instance id
			aiAgentInstanceID = m_instanceConter++;
#if UNITY_EDITOR
			m_dOriginalInstanceID = aiAgentInstanceID;
#endif

			//Table初期化
			foreach (AITable table in m_aiTables)
				table.Start(this);

			foreach (var e in m_notTableMembers)
				if (e.function != null) e.function.StartAIFunction(this, null);

			//Errorチェック
#if UNITY_EDITOR
			if (m_aiTables.Length <= m_beginTableIndex)
			{
				Debug.LogError("Error!! AIAgent->Start \n Invalid begin table index. index: " + m_beginTableIndex);
				return;
			}
#endif

			//初期実行関数クラス取得
			nowFunction = m_aiTables[m_beginTableIndex].SelectionFunction();

			//Errorチェック
#if UNITY_EDITOR
			if (nowFunction == null)
			{
				Debug.LogError("Error!! AIAgent->Start \n nowFunction == null");
				return;
			}
#endif

			//Cloneなら名前変更
			if (gameObject.name.Contains("(Clone)"))
			{
				gameObject.name =
					gameObject.name.Replace("(Clone)", "") + " AgentInstanceID : " + aiAgentInstanceID;
			}
			//instanceIDを名前に追加
			else
			{
				gameObject.name += " AgentInstanceID : " + aiAgentInstanceID;
			}

			//debug only, 関数表示用文字列初期化
#if UNITY_EDITOR
			m_dNowTable = m_aiTables[m_beginTableIndex].tableName;
			m_dNowFunction = nowFunction.functionName;

			if (m_aiTables[m_beginTableIndex].condition != null)
				m_dNowTableCondition = m_aiTables[m_beginTableIndex].condition.dConditionName;
			else if (m_aiTables[m_beginTableIndex].isConditionAlwaysTrueSelf)
				m_dNowTableCondition = "Always true";
			else if (m_aiTables[m_beginTableIndex].isConditionAlwaysFalseSelf)
				m_dNowTableCondition = "Always false";
			else
				m_dNowTableCondition = "Not condition";
#endif

			//関数Start
			nowFunction.timer.Start();
			nowFunction.AIBegin(null);
			m_isBeginCallBackCompleted = true;
		}

		/// <summary> [Update] </summary>
		protected void Update()
		{
#if UNITY_EDITOR
			if (nowFunction == null)
			{
				Debug.LogError("Error!! AIAgent->FixedUpdate now function == null");
				return;
			}
#endif

			//Callback未呼び出しならTimer初期化, Callback呼び出し, フラグ初期化
			if (!m_isBeginCallBackCompleted)
			{
				nowFunction.timer.Start();
				nowFunction.AIBegin(m_oldFunction);
				m_isBeginCallBackCompleted = true;
			}
			//Update
			nowFunction.AIUpdate(m_updateIdentifier);
		}

		/// <summary> [FixedUpdate] </summary>
		protected void FixedUpdate()
		{
#if UNITY_EDITOR
			if (nowFunction == null)
			{
				Debug.LogError("Error!! AIAgent->FixedUpdate now function == null");
				return;
			}
#endif

			//Callback未呼び出しならTimer初期化, Callback呼び出し, フラグ初期化
			if (!m_isBeginCallBackCompleted)
			{
				nowFunction.timer.Start();
				nowFunction.AIBegin(m_oldFunction);
				m_isBeginCallBackCompleted = true;
			}
			//Update
			nowFunction.AIFixedUpdate(m_updateIdentifier);
		}

		/// <summary> [LateUpdate] </summary>
		protected void LateUpdate()
		{
#if UNITY_EDITOR
			if (nowFunction == null)
			{
				Debug.LogError("Error!! AIAgent->FixedUpdate now function == null");
				return;
			}
#endif

			//Callback未呼び出しならTimer初期化, Callback呼び出し, フラグ初期化
			if (!m_isBeginCallBackCompleted)
			{
				nowFunction.timer.Start();
				nowFunction.AIBegin(m_oldFunction);
				m_isBeginCallBackCompleted = true;
			}
			//Update
			nowFunction.AILateUpdate(m_updateIdentifier);
		}
#if UNITY_EDITOR
		/// <summary> [OnDrawGizmos] </summary>
		void OnDrawGizmos()
		{
			if (m_aiTables == null) return;

			for (int i = 0; i < m_aiTables.Length; ++i)
				m_aiTables[i].DNameSet();
			for (int i = 0; i < m_notTableMembers.Length; ++i)
				m_notTableMembers[i].DNameSet();
		}
#endif
	}
}