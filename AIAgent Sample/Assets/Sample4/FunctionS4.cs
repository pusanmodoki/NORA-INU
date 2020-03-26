using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class FunctionS4 : AIComponent.BaseAIFunction
{
	//Startみたいなもん
	public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
	{
		//元クラスみたらだいたいわかるけど、よくつかうメンバを紹介


		//このAIAgentに紐付いているNavMeshAgent
		var s0 = navMeshAgent;
		//紐付いているAIAgent
		var s1 = aiAgent;
		//Begin実行からの秒数が入っている
		var s2 = timer;

		//AIAgentはRigidBodyと同じく独自に座標をもってる。そいつをいじるサポート関数
		//第一引数はtrueだと移動可能に, falseだと移動しなくなる
		//第二引数はtrueだと今の座標へワープを行う
		SetUpdatePosition(true);

		//現在実行中の関数, 外部からみたりね
		var s3 = aiAgent.nowFunction;
		//紐付いているRigidBody
		var s4 = aiAgent.rigidBody;
		//trueなら移動するようになる
		var s5 = aiAgent.navMeshUpdatePosition;
		//trueなら移動時回転するようになる
		var s6 = aiAgent.navMeshUpdateRotation;

		//関数を名前で検索する
		var s7 = aiAgent.FindFunction("", "");

		//RigidBodyのFreezeを一括設定してくれるヘルパー
		//結構スクリプトから設定するのだるいからね
		aiAgent.SetRigitFreezeAll();
		aiAgent.SetRigitFreezeExcludingPositionXZ();

		//加速度
		var s8 = navMeshAgent.acceleration;
		//速度
		var s9 = navMeshAgent.speed;

		//割り込み実行, 実行中のものは終了
		//aiAgent.ForceSpecifyFunction(null);
		//再割当て, 実行中のものは終了
		//aiAgent.AllocateFunction();
	}

	//OnDisableみたいなもん
	public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
	{
	}

	//Updateみたいなもん
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
	}

	//FixedUpdateみたいなもん, 継承はしなくてもよい
	public override void AIFixedUpdate(UpdateIdentifier updateIdentifier) { }
	//LateUpdateみたいなもん, 継承はしなくてもよい
	public override void AILateUpdate(UpdateIdentifier updateIdentifier) { }
}
