//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Components 
/// </summary>
namespace AIComponent
{
    /// <summary>
    /// AIAgentで呼び出すコールバック関数を格納したBaseAIFunction class
    /// </summary>
    public abstract class BaseAIFunction : MonoBehaviour
    {
        /// <summary>
        /// EndAIFunctionに必要なUpdateIdentifier class
        /// 終了関数なので、敢えて使いにくくしている
        /// </summary>
        public class UpdateIdentifier
        {
        };

        /// <summary> NavMeshAgent </summary>
        public UnityEngine.AI.NavMeshAgent navMeshAgent { get; private set; } = null;
        /// <summary> AIAgent->AITable->this </summary>
        public AIAgent aiAgent { get; private set; } = null;
        /// <summary> AITable->this </summary>
        public AITable aiTable { get; private set; } = null;

        /// <summary> この関数クラスが所属するテーブルの名前 </summary>
        public string tableName { get; private set; } = null;
        /// <summary> この関数クラスの名前 </summary>
        public string functionName { get { return m_functionName; } }
        /// <summary> 実行時にStartされるTimer </summary>
        public Timer timer { get; private set; } = new Timer();
        /// <summary> instanceID </summary>
        public int instanceFunctionID { get; private set; } = -1;

		/// <summary>Adjust SetUpdatePosition</summary>
		static readonly Vector3 m_cAdjustSetUpdatePosition = new Vector3(0.0f, 0.1f, 0.0f);
		/// <summary>AI function instance id counter</summary>
		static int m_instanceIDCounter = 0;
		
        /// <summary> Function Name </summary>
        [SerializeField, Tooltip("Function name")]
        string m_functionName = "AI function";

        /// <summary>
        /// [StartAIFunction]
        /// AIAgentで呼び出すStart関数
        /// 引数1: 自身の所属するAIAgent
        /// 引数2: 自身の所属するAITable
        /// </summary>
        public virtual void StartAIFunction(AIAgent aiAgent, AITable aiTable)
        {
            this.aiAgent = aiAgent;
            this.aiTable = aiTable;
            navMeshAgent = aiAgent.navMeshAgent;
            tableName = aiTable != null ? aiTable.tableName : "Not table";
            instanceFunctionID = m_instanceIDCounter++;
        }

		/// <summary>
		/// [AIBegin]
		/// 関数初回実行時に呼ばれるコールバック関数
		/// 引数1: 通常実行→終了する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
		/// </summary>
		public abstract void AIBegin(BaseAIFunction beforeFunction);
		/// <summary>
		/// [AIEnd]
		/// 関数が実行登録を解除される際に呼ばれるコールバック関数
		/// 引数1: 通常実行→次回実行する関数, 割り込み実行の場合→現在の通常実行関数, 存在しない場合null
		/// </summary>
		public abstract void AIEnd(BaseAIFunction nextFunction);

		/// <summary>
		/// [AIUpdate]
		/// Updateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
		/// 引数1: 更新識別子
		/// </summary>
		public abstract void AIUpdate(UpdateIdentifier updateIdentifier);
		/// <summary>
		/// [AIFixedUpdate]
		/// FixedUpdateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
		/// 引数1: 更新識別子
		/// </summary>
		public virtual void AIFixedUpdate(UpdateIdentifier updateIdentifier) { }
		/// <summary>
		/// [AILateUpdate]
		/// LateUpdateフレームに呼ばれるコールバック関数, EndAIFunctionを呼び出す場合引数1が必要
		/// 引数1: 更新識別子
		/// </summary>
		public virtual void AILateUpdate(UpdateIdentifier updateIdentifier) { }

        /// <summary>
        /// [EndAIFunction]
        /// 実行中の関数を登録解除する, 複数実行されている場合実行中のもののみ解除される
        /// 引数1: 更新識別子
        /// </summary>
        public void EndAIFunction(UpdateIdentifier updateIdentifier)
        {
			//停止
			timer.Stop();

            //関数新規割り当て
            aiAgent.AllocateFunction();
        }
		
		/// <summary>
		/// [SetUpdatePosition]
		/// NavMeshAgent->updatePosition = isSet, Warp(transform.position)
		/// 引数1: Set value
		/// 引数2: Warpさせるか (させると経路情報がリセットされる危険があります), default = true
		/// </summary>
		protected void SetUpdatePosition(bool isSet, bool isWarp = true)
		{
			if (isSet == navMeshAgent.updatePosition)
				return;

			Vector3 position = transform.position;
			position.y += navMeshAgent.baseOffset;

			if (isWarp) navMeshAgent.Warp(position);
			navMeshAgent.updatePosition = isSet;
		}
		/// <summary>
		/// [SetUpdatePosition]
		/// NavMeshAgent->updatePosition = isSet, Warp(newPositoin)
		/// 引数1: newPosition
		/// 引数2: Set value
		/// 引数3: Warpさせるか (させると経路情報がリセットされる危険があります), default = true
		/// </summary>
		protected void SetUpdatePosition(Vector3 newPositoin, bool isSet, bool isWarp = true)
		{
			if (isSet == navMeshAgent.updatePosition)
				return;

			if (isWarp) navMeshAgent.Warp(newPositoin);
			navMeshAgent.updatePosition = isSet;
		}
	}
}