using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// OffMeshLinkの操作を行うDogOffMeshLinkController
/// 参考にしたサイト: https://www.urablog.xyz/entry/2017/10/16/145350
/// </summary>
[System.Serializable]
public class DogOffMeshLinkController
{
	/// <summary>Jumpにかかる時間</summary>
	[Header("Default Jump Settings"), SerializeField, Tooltip("Jumpにかかる時間")]
	float m_jumpSeconds = 1.5f;
	/// <summary>Jump中の回転速度時間</summary>
	[SerializeField, Tooltip("Jump中の回転速度時間")]
	float m_rotationSpeed = 3.0f;

	/// <summary>This agent</summary>
	DogAIAgent m_thisAgent = null;
	/// <summary>This NavMeshAgent</summary>
	NavMeshAgent m_navMeshAgent = null;
	/// <summary>This Transform</summary>
	Transform m_transform = null;
	/// <summary>This Rigidbody</summary>
	Rigidbody m_rigidBody = null;
	/// <summary>Timer</summary>
	Timer m_timer = new Timer();
	/// <summary>Point BaseSpecialOffMeshLink</summary>
	BaseUniqueOffMeshLink m_pointUniqueOffMeshLink = null;
	/// <summary>Add BaseSpecialOffMeshLink</summary>
	BaseUniqueOffMeshLink m_addUniqueOffMeshLink = null;
	/// <summary>回転</summary>
	Quaternion lookRotation = Quaternion.identity;
	/// <summary>ジャンプ目標座標</summary>
	Vector3 m_targetPosition = Vector3.zero;
	/// <summary>This Transform.position</summary>
	Vector3 m_position = Vector3.zero;
	/// <summary>ジャンプ目標座標</summary>
	bool m_isStartOffMeshLink = false;
	/// <summary>デフォルト動作？</summary>
	bool m_isDefault = false;
	/// <summary>コルーチン実行中</summary>
	bool m_isCoroutine = false;

	/// <summary>
	/// [InitContoroller]
	/// 初期化を行う
	/// 引数1: This Agent
	/// </summary>
	public void InitContoroller(DogAIAgent thisAgent)
	{
		m_thisAgent = thisAgent;
		m_navMeshAgent = thisAgent.navMeshAgent;
		m_transform = thisAgent.transform;
		m_rigidBody = thisAgent.rigidBody;

		m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		m_rigidBody.isKinematic = true;
		m_rigidBody.useGravity = false;
	}

	/// <summary>
	/// [CalculateNavMeshLinkEndPoint]
	/// NavMeshLinkの場合EndPointが信用できないので自分で求める
	/// </summary>
	public static Vector3 CalculateNavMeshLinkEndPoint(NavMeshLink navMeshLink, ref Vector3 position, ref Vector3 startPoint, ref Vector3 endPoint)
	{
		//result
		Vector3 result = Vector3.zero;
		//endPointの線分に沿ったVector
		Vector3 right = Vector3.Cross(new Vector3(endPoint.x - startPoint.x, 0.0f, endPoint.z - startPoint.z).normalized, Vector3.up);
		//endPoint segment start
		Vector3 start = (endPoint + -right * navMeshLink.width);
		//endPoint segment end
		Vector3 end = (endPoint + right * navMeshLink.width);
		//endPoint start -> this position
		Vector3 startToThis = position - start;
		//endPoint start -> endPoint end
		Vector3 startToEnd = end - start;

		//dot
		float dot = Vector3.Dot(startToThis, startToEnd);

		//鈍角な場合startが一番近い
		if (dot <= 0.0f) return start;

		//startToEnd magnitude
		float startToEndMagnitude = startToEnd.magnitude;
		//|vec1||vec2|cosθ -> |vec1|cosθ(startToThis projection)
		float projection = dot / startToEndMagnitude;

		//射影がstartToEndMagnitude未満であれば線分上の点が最も近い
		//start + startToEnd * 倍率
		if (projection < startToEndMagnitude)
			return start + startToEnd * (projection / startToEndMagnitude);
		//そうでなければendが一番近い
		else
			return end;
	}

	/// <summary>[Update]</summary>
	public void Update()
	{
		//OffMeshLink上にいなければ終了
		if (!m_navMeshAgent.isOnOffMeshLink)
			return;

		//初期化
		if (!m_isStartOffMeshLink && !m_isCoroutine)
		{
			m_isCoroutine = true;
			m_navMeshAgent.isStopped = true;
			m_thisAgent.StartCoroutine(InitCoroutine());
		}

		//デフォルト動作
		if (m_isDefault & m_isStartOffMeshLink)
		{
			//回転
			m_transform.rotation = Quaternion.Slerp(m_transform.rotation, lookRotation, m_rotationSpeed * Time.deltaTime);

			//一定時間経過で終了
			if (m_timer.elapasedTime >= m_jumpSeconds)
				EndOffMeshLink();
		}
		else if (!m_isDefault & m_isStartOffMeshLink)
		{
			if (!m_addUniqueOffMeshLink.CalledUpdateOffMeshLink())
			{
				m_addUniqueOffMeshLink.Unlink();
				Object.Destroy(m_addUniqueOffMeshLink);
				m_addUniqueOffMeshLink = null;

				//登録解除
				m_pointUniqueOffMeshLink.UnacquisitionRightToUse();
				m_pointUniqueOffMeshLink = null;

				EndOffMeshLink();
			}
		}
	}
	/// <summary>[FixedUpdate]</summary>
	public void FixedUpdate()
	{
		if (m_isDefault && m_isStartOffMeshLink && m_timer.elapasedTime >= m_jumpSeconds)
			EndOffMeshLink();
		else if (!m_isDefault && m_isStartOffMeshLink && !m_addUniqueOffMeshLink.CalledFixedUpdateOffMeshLink())
		{
			m_addUniqueOffMeshLink.Unlink();
			Object.Destroy(m_addUniqueOffMeshLink);
			m_addUniqueOffMeshLink = null;

			//登録解除
			m_pointUniqueOffMeshLink.UnacquisitionRightToUse();
			m_pointUniqueOffMeshLink = null;

			EndOffMeshLink();
		}
	}

	IEnumerator InitCoroutine()
	{
		//初期地点, 完了地点
		Vector3 startPoint, endPoint;
		//NavMeshLink
		NavMeshLink navMeshLink = m_navMeshAgent.currentOffMeshLinkData.offMeshLink == null ?
			(m_navMeshAgent.navMeshOwner as NavMeshLink) : null;

		//offMeshLink or navMeshLink transform
		Transform linkTransform = navMeshLink == null ?
			m_navMeshAgent.currentOffMeshLinkData.offMeshLink.transform : navMeshLink.transform;
		//コンポーネント取得
		m_pointUniqueOffMeshLink = linkTransform.GetComponent<BaseUniqueOffMeshLink>();

		//コンポーネントがあった場合特殊動作, 使用できない場合待ちます
		while(m_pointUniqueOffMeshLink != null)
		{
			if (!m_navMeshAgent.isOnOffMeshLink)
			{
				m_pointUniqueOffMeshLink = null;
				yield break;
			}

			if (m_pointUniqueOffMeshLink.AcquisitionRightToUse())
				break;
			else
				yield return null;
		}

		//position取得, フラグ設定
		m_position = m_transform.position;
		m_isStartOffMeshLink = true;

		//TargetPosition設定
		m_targetPosition = m_navMeshAgent.currentOffMeshLinkData.endPos;
		//Y調整
		m_targetPosition.y += m_position.y - m_navMeshAgent.currentOffMeshLinkData.startPos.y;

		//OffMeshLink??
		if (navMeshLink == null)
		{
			startPoint = m_navMeshAgent.currentOffMeshLinkData.offMeshLink.startTransform.position;
			endPoint = m_navMeshAgent.currentOffMeshLinkData.offMeshLink.endTransform.position;
		}
		//NavMeshLink??
		else
		{
			startPoint = navMeshLink.transform.LocalToWorldPosition(navMeshLink.startPoint);
			endPoint = navMeshLink.transform.LocalToWorldPosition(navMeshLink.endPoint);
		}

		//距離でどっちが初期地点か判定
		if ((startPoint - m_position).sqrMagnitude > (endPoint - m_position).sqrMagnitude)
		{
			Vector3 temp = startPoint;
			startPoint = endPoint;
			endPoint = temp;
		}

		//NavMeshLinkの場合EndPointが信用できないので自分で求める
		if (navMeshLink != null && navMeshLink.width > 0.0f)
		{
			endPoint = CalculateNavMeshLinkEndPoint(navMeshLink, ref m_position, ref startPoint, ref endPoint);
			m_targetPosition.x = endPoint.x;
			m_targetPosition.z = endPoint.z;
		}

		//コンポーネントがない場合デフォルト動作
		if (m_pointUniqueOffMeshLink == null)
		{
			m_isDefault = true;
			StartDefaultOffMeshLink(ref startPoint, ref endPoint);
		}
		//コンポーネントがあった場合特殊動作
		else
		{
			m_isDefault = false;
			var type = m_pointUniqueOffMeshLink.GetType();
			m_addUniqueOffMeshLink = m_transform.gameObject.AddComponent(type) as BaseUniqueOffMeshLink;
			m_addUniqueOffMeshLink.CopyComponent(m_pointUniqueOffMeshLink, type);

			m_addUniqueOffMeshLink.Link(m_transform, linkTransform, m_rigidBody, m_navMeshAgent,
				 m_thisAgent.groundFlag, ref startPoint, ref endPoint, ref m_targetPosition);
		}
		m_isCoroutine = false;
		yield break;
	}

	/// <summary>
	/// [StartDefaultOffMeshLink]
	/// デフォルト動作を実行する
	/// </summary>
	void StartDefaultOffMeshLink(ref Vector3 startPoint, ref Vector3 endPoint)
	{  
		//RigidBodyのKinematic解除
		m_rigidBody.isKinematic = false;
		m_rigidBody.useGravity = true;
		m_rigidBody.velocity = Vector3.zero;

		//NavMesh停止, 初期化フラグtrueに
		m_navMeshAgent.isStopped = true;
		m_navMeshAgent.updateRotation = false;

		//向くべき回転を設定
		startPoint.y = endPoint.y = 0.0f;
		lookRotation = Quaternion.LookRotation((endPoint - startPoint).normalized);

		//ジャンプ実行
		JumpExecution();
		//タイマースタート
		m_timer.Start();
	}
	/// <summary>
	/// [EndOffMeshLink]
	/// OffMeshLink処理を終了する
	/// </summary>
	void EndOffMeshLink()
	{
		//フラグ初期化, タイマーストップ
		m_isStartOffMeshLink = false;
		m_timer.Stop();
		//RigidBodyをKinematicに、Velocity = 0
		m_rigidBody.isKinematic = true;
		m_rigidBody.useGravity = false;
		m_rigidBody.velocity = Vector3.zero;
		//OffMeshLink終了, Stop解除
		m_navMeshAgent.CompleteOffMeshLink();
		m_navMeshAgent.isStopped = false;
		m_navMeshAgent.updateRotation = true;

		//destination保存
		Vector3 destination = m_navMeshAgent.destination;
		//Warp
		NavMeshHit hit = default;
		NavMesh.SamplePosition(m_transform.position, out hit, 100, NavMesh.AllAreas);
		m_navMeshAgent.Warp(hit.position);
		//destination再設定
		m_navMeshAgent.destination = destination;
	}

	/// <summary>
	/// [JumpExecution]
	/// ジャンプを実行する
	/// </summary>
	void JumpExecution()
	{
		//放物線計算に使うdetail を取得
		Vector2 detail = CalculateDetail();
		//スピードを計算
		float speed = CalculateSpeed(ref detail);
		//角度を計算
		float angle = CalculateAngle(ref detail);

		//スピードが0以下であればジャンプしない
		if (speed <= 0.0f)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogOffMeshLinkController->JumpExecution\nJump failed");
#endif
			return;
		}

		//AddForce
		m_rigidBody.AddForce(
			CalculateForce(speed, angle) * m_rigidBody.mass, ForceMode.Impulse);
	}
	/// <summary>
	/// [CalculateDetail]
	/// 放物線計算に使用するDetailを計算する
	/// </summary>
	Vector2 CalculateDetail()
	{
		float distance = Vector2.Distance(new Vector2(m_targetPosition.x, m_targetPosition.z),
			new Vector2(m_position.x, m_position.z));

		return new Vector2(distance / m_jumpSeconds,
			(m_targetPosition.y - m_position.y) / m_jumpSeconds + (-Physics.gravity.y * m_jumpSeconds) / 2);
	}
	/// <summary>
	/// [CalculateSpeed]
	/// 放物線計算に使用するスピードを計算する
	/// 引数1: detail
	/// </summary>
	float CalculateSpeed(ref Vector2 detail)
	{
		//0以下であれば終了
		if (m_jumpSeconds <= 0.0f) return 0.0f;

		//dot計算
		float dot = detail.x * detail.x + detail.y * detail.y;

		//平方根計算
		if (dot > 0.0f) return Mathf.Sqrt(dot);
		else return 0.0f;
	}
	/// <summary>
	/// [CalculateSpeed]
	/// 放物線計算に使用する角度を計算する
	/// 引数1: detail
	/// </summary>
	float CalculateAngle(ref Vector2 detail)
	{
		//Atan2
		return Mathf.Atan2(detail.y, detail.x) * Mathf.Rad2Deg;
	}
	/// <summary>
	/// [CalculateForce]
	/// RigidBodyに与える力を計算する
	/// 引数1: speed
	/// 引数1: angle
	/// </summary>
	Vector3 CalculateForce(float speed, float angle)
	{
		//目標座標
		Vector3 targetPosition = m_targetPosition;
		//XZ平面の値がほしいのでYを0にする
		m_position.y = targetPosition.y = 0.0f;
		
		//yaw angle * x angle * x speed
		return Quaternion.FromToRotation(Vector3.right, (targetPosition - m_position).normalized)
			* Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(speed, 0.0f, 0.0f);
	}
}
