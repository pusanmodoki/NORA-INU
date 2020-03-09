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
		public BaseAIFunction[] notTableMembers { get { return m_notTableMembers; } }
		/// <summary> NavMeshAgent get only property </summary>
		public NavMeshAgent navMeshAgent { get { return m_navMeshAgent; } }
		/// <summary> RigitBody get only property </summary>
		public Rigidbody rigidBody { get { return m_rigidbody; } }
		/// <summary> User data transform get only property </summary>
		public Transform userDataTransform { get { return m_userDataTransform; } }
		/// <summary> Now function </summary>
		public BaseAIFunction nowFunction { get; private set; } = null;
		/// <summary> Original instanceID</summary>
		public int aiAgentInstanceID { get; private set; } = -1;
		/// <summary> Prefab instanceID</summary>
		public int prefabInstanceID { get; private set; } = -1;
		/// <summary> Now running parallel index</summary>
		public int nowRunningParallelIndex { get; private set; } = -1;
		/// <summary> NavMeshAgent -> updatePosition get and set property </summary>
		public bool navMeshUpdatePosition { get { return m_navMeshAgent.updatePosition; } set { m_navMeshAgent.updatePosition = value; } }
		/// <summary> NavMeshAgent -> updateRotation get and set property </summary>
		public bool navMeshUpdateRotation { get { return m_navMeshAgent.updateRotation; } set { m_navMeshAgent.updateRotation = value; } }

		/// <summary> This nav mesh agent </summary>
		[SerializeField, Tooltip("This nav mesh agent")]
		NavMeshAgent m_navMeshAgent = null;
		/// <summary> This rigid body </summary>
		[SerializeField, Tooltip("This rigid body")]
		Rigidbody m_rigidbody = null;
		/// <summary> Startup ai table </summary>
		[SerializeField, Tooltip("Startup ai table")]
		int m_beginTableIndex = 0;

		/// <summary> User data transform </summary>
		[SerializeField, Tooltip("User data transform"), Space]
		Transform m_userDataTransform = null;
		/// <summary> AITable list </summary>
		[SerializeField, Tooltip("AI table list"), Space]
		AITable[] m_aiTables = null;
		/// <summary> AITable list </summary>
		[SerializeField, Tooltip("Not table member functions"), Space]
		BaseAIFunction[] m_notTableMembers = null;

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

		/// <summary> Instancecounter </summary>
		static int m_instanceConter = 0;

		/// <summary> ひとつ前の実行設定関数クラス </summary>
		BaseAIFunction m_oldFunction = null;
		/// <summary> Function->End用 (敢えて少し実行しにくいようにしている) </summary>
		BaseAIFunction.UpdateIdentifier m_updateIdentifier = new BaseAIFunction.UpdateIdentifier();
		/// <summary> 割り込み関数用リスト </summary>
		List<BaseAIFunction> m_parallelFunctions = new List<BaseAIFunction>();
		
		/// <summary> 割り込み関数, 単一動作設定値 </summary>
		bool m_isParallelSingleActoin = false;
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
					//成功すれば終了側Callback呼び出し, フラグ初期化
					if (nowFunction != null)
					{
						m_oldFunction = oldSave;
						m_oldFunction?.AIEnd(nowFunction, false);
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
			m_oldFunction?.AIEnd(nowFunction, false);
		}


		/// <summary>
		/// [ParallelSettingFunction]
		/// 関数クラスを割り込み設定する
		/// 割り込み設定したクラスは通常実行クラスと並列実行される
		/// 引数1: 割り込ませる関数
		/// 引数2: 単一動作にさせるか否か. trueにすると実行中のものは終了する
		///				 falseにすると別の関数がSingle実行中の場合割り当てられない, default = false
		/// </summary>
		public void ParallelSettingFunction(BaseAIFunction settingFunction, bool isSingleAction = false)
		{
			//シングル動作中でフラグがシングル動作でない場合は終了
			if (m_isParallelSingleActoin & !isSingleAction) return;

			//シングル動作ならリストクリア
			if (isSingleAction)
			{
				foreach (var e in m_parallelFunctions)
					e.AIEnd(nowFunction, true);
				m_parallelFunctions.Clear();
			}

			//リストに関数クラス追加
			m_parallelFunctions.Add(settingFunction);
			//タイマースタート
			settingFunction.timer.Start();
			//割り込み側のCallback呼び出し
			settingFunction.AIBegin(nowFunction, true);

			//シングル動作か否かを保存
			m_isParallelSingleActoin = isSingleAction;
		}
		/// <summary>
		/// [EndParallelFunctionAll]
		/// 並列関数を全て終了させる
		/// </summary>
		public void EndParallelFunctionAll()
		{
			foreach (var e in m_parallelFunctions)
				e.AIEnd(nowFunction, true);
			m_parallelFunctions.Clear();
		}
		/// <summary>
		/// [EndParallelFunction]
		/// 並列関数を全て終了させる
		/// 引数1: index
		/// </summary>
		public void EndParallelFunction(int index)
		{
			if (index >= 0 && index < m_parallelFunctions.Count)
			{
				m_parallelFunctions[index].AIEnd(nowFunction, true);
				m_parallelFunctions.RemoveAt(index);
			}
		}

		/// <summary>
		/// [SetUserDataTransform]
		/// User用TransformをSet
		/// 引数1: Transform
		/// </summary>
		public void SetUserDataTransform(Transform transform)
		{
			m_userDataTransform = transform;
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
		void Awake()
		{
			if (prefabInstanceID == -1)
				prefabInstanceID = GetInstanceID();
		}
		/// <summary> [Start] </summary>
		void Start()
		{
			//Set instance id
			aiAgentInstanceID = m_instanceConter++;
#if UNITY_EDITOR
			m_dOriginalInstanceID = aiAgentInstanceID;
#endif

			//Table初期化
			foreach (AITable table in m_aiTables)
				table.Start(this);

			//初期実行関数クラス取得
			nowFunction = m_aiTables[m_beginTableIndex].SelectionFunction();
			nowFunction.timer.Start();
			nowFunction.AIBegin(null, false);
			m_isBeginCallBackCompleted = true;

			//Cloneなら名前変更
			if (gameObject.name.Contains("(Clone)"))
			{
				gameObject.name =
					gameObject.name.Replace("(Clone)", "") + " AgentInstanceID : " + aiAgentInstanceID;
			}
			//instanceIDを名前に追加
			else
			{
				gameObject.name += "AgentInstanceID : " + aiAgentInstanceID;
			}
		}

		/// <summary> [Update] </summary>
		void Update()
		{
			//debug only, 関数表示用文字列初期化
#if UNITY_EDITOR
			m_dNowTable = nowFunction.tableName;
			m_dNowFunction = nowFunction.functionName;
			m_dNowTableCondition = nowFunction.aiTable.condition.dConsitionName;
#endif

			//Callback未呼び出しならTimer初期化, Callback呼び出し, フラグ初期化
			if (!m_isBeginCallBackCompleted)
			{
				nowFunction.timer.Start();
				nowFunction.AIBegin(m_oldFunction, false);
				m_isBeginCallBackCompleted = true;
			}
			//Update
			nowFunction.AIUpdate(m_updateIdentifier.Call(false));

			//parallel function 実行ループ
			for (nowRunningParallelIndex = 0; 
				nowRunningParallelIndex < m_parallelFunctions.Count; ++nowRunningParallelIndex)
			{
				m_parallelFunctions[nowRunningParallelIndex].AIUpdate(m_updateIdentifier.Call(true));
			}
			nowRunningParallelIndex = -1;
		}
		/// <summary> [FixedUpdate] </summary>
		void FixedUpdate()
		{
			//Callback未呼び出しならTimer初期化, Callback呼び出し, フラグ初期化
			if (!m_isBeginCallBackCompleted)
			{
				nowFunction.timer.Start();
				nowFunction.AIBegin(m_oldFunction, false);
				m_isBeginCallBackCompleted = true;
			}
			//Update
			nowFunction.AIFixedUpdate(m_updateIdentifier.Call(false));

			//parallel function 実行ループ
			for (nowRunningParallelIndex = 0;
				nowRunningParallelIndex < m_parallelFunctions.Count; ++nowRunningParallelIndex)
			{
				m_parallelFunctions[nowRunningParallelIndex].AIFixedUpdate(m_updateIdentifier.Call(true));
			}
			nowRunningParallelIndex = -1;
		}
		/// <summary> [LateUpdate] </summary>
		void LateUpdate()
		{
			//Callback未呼び出しならTimer初期化, Callback呼び出し, フラグ初期化
			if (!m_isBeginCallBackCompleted)
			{
				nowFunction.timer.Start();
				nowFunction.AIBegin(m_oldFunction, false);
				m_isBeginCallBackCompleted = true;
			}
			//Update
			nowFunction.AILateUpdate(m_updateIdentifier.Call(false));

			//parallel function 実行ループ
			for (nowRunningParallelIndex = 0;
				nowRunningParallelIndex < m_parallelFunctions.Count; ++nowRunningParallelIndex)
			{
				m_parallelFunctions[nowRunningParallelIndex].AILateUpdate(m_updateIdentifier.Call(true));
			}
			nowRunningParallelIndex = -1;
		}
#if UNITY_EDITOR
		/// <summary> [OnDrawGizmos] </summary>
		void OnDrawGizmos()
		{
			if (m_aiTables == null) return;

			for (int i = 0; i < m_aiTables.Length; ++i)
				m_aiTables[i].DNameSet();
		}
#endif
	}
}