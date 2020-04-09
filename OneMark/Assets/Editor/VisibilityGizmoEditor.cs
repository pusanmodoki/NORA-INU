using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// VisibilityのGizmo描画用VisibilityGizmoEditor class
    /// </summary>
    public static class VisibilityGizmoEditor
    {
        /// <summary>
        /// Mesh保存用構造体
        /// </summary>
        public struct SectorMesh
        {
            /// <summary>[コンストラクタ]</summary>
            public SectorMesh(Mesh mesh, float angle, int id)
            {
                this.mesh = mesh;
                this.angle = angle;
                this.id = id;
                this.counter = 1;
            }
            
            /// <summary>return: Destroy OK -> true</summary>
            public bool isDestroyReady { get { return counter == 0; } }

            /// <summary>Mesh</summary>
            public Mesh mesh;
            /// <summary>angle</summary>
            public float angle;
            /// <summary>this id</summary>
            public int id;
            /// <summary>ref counter</summary>
            public int counter;
        }


        /// <summary>Vertex = Count * 3</summary>
        static readonly int m_cTriangleCount = 15;
		/// <summary>Mesh Color->NotHit</summary>
		static readonly Color m_cOverlapColor = new Color(0.1f, 0.1f, 0.7f, 0.5f);
		/// <summary>RayCast Color->Hit</summary>
		static readonly Color m_cRayColorHit = new Color(1.0f, 0.15f, 0.1f, 0.8f);
        /// <summary>RayCast Color->NotHit</summary>
        static readonly Color m_cRayColorNotHit = new Color(0.8f, 0.8f, 0.05f, 0.8f);
        /// <summary>Mesh Color->Hit</summary>
        static readonly Color m_cMeshColorHit = new Color(1.0f, 0.25f, 0.2f, 0.5f);
        /// <summary>Mesh Color->NotHit</summary>
        static readonly Color m_cMeshColorNotHit = new Color(0.7f, 0.7f, 0.1f, 0.5f);
        /// <summary>Mesh Destroy Check Interval</summary>
        static readonly double m_cCheckDestroyTime = 3.0f;
		//地面に接触する可能性を考慮し描画位置を調整するために使用
		static readonly Vector3 m_cAdjustUp = Vector3.up * 0.1f;

		/// <summary>SectorMesh List</summary>
		static List<SectorMesh> m_meshs = new List<SectorMesh>();
        /// <summary>meshID Counter</summary>
        static int m_meshID = 0;
        /// <summary>Mesh Destroy Check</summary>
        static double m_startTime = 0;

        /// <summary>
        /// [DrawGizmos]
        /// GIzmo描画
        /// 引数1: AIAgentVisibility
        /// 引数2: GizmoType
        /// </summary>
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawGizmos(AIComponent.AIVisibility visibility, GizmoType gizmoType)
		{
			//null?
			if (visibility.aiAgent == null) return;
            //draw?
            if (!visibility.dIsDrawGizmos) return;

            //削除確認するか経過時間で確認
            if (m_startTime + m_cCheckDestroyTime < EditorApplication.timeSinceStartup)
            {
                m_startTime = EditorApplication.timeSinceStartup;
                //削除ループ
                for (int i = 0; i < m_meshs.Count;)
                {
                    //削除可能なら削除
                    if (m_meshs[i].isDestroyReady)
                        m_meshs.RemoveAt(i);
                    else
                        i++;
                }
            }

            //Distanceがおかしければ描画しない
            if (visibility.distanceVisibility <= 0.0f)
                return;

            //描画に反映されないため実行中以外はこちらで更新を行う
            if (!EditorApplication.isPlaying || EditorApplication.isPaused)
                visibility.IsHitVisibility(true);

            //呼び出しコスト削減
            Transform transform = visibility.transform;
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
			//Center
			Vector3 raycastCenter, overlapCenter;
			visibility.CalculateCenters(position, out raycastCenter, out overlapCenter);
			//レイ開始地点
			Vector3 rayStart = raycastCenter + m_cAdjustUp;

			//Overlap描画
			Gizmos.color = m_cOverlapColor;
			Gizmos.DrawWireSphere(overlapCenter, visibility.overlapRadius);

			//視界ヒット
			if (visibility.dIsHit)
            {
                if (visibility.lookTarget != null)
                {
                    //レイ終了地点
                    Vector3 rayEnd = (visibility.lookTarget.position - raycastCenter).normalized * visibility.raycastHit.distance;
                    //レイ用に色変更しレイ描画
                    Gizmos.color = m_cRayColorHit;
                    Gizmos.DrawLine(rayStart, rayStart + rayEnd);
                }
                //メッシュ用に色変更
                Gizmos.color = m_cMeshColorHit;
            }
            //視界ヒットしていない
            else
            {
                if (visibility.lookTarget != null)
                {
                    //レイ終了地点
                    Vector3 rayEnd = (visibility.lookTarget.position - raycastCenter).normalized * visibility.distanceVisibility;
                    //レイ用に色変更しレイ描画
                    Gizmos.color = m_cRayColorNotHit;
                    Gizmos.DrawLine(rayStart, rayStart + rayEnd);
                }
                //メッシュ用に色変更
                Gizmos.color = m_cMeshColorNotHit;
            }

            //visibilityのmeshがnull -> 未設定 or 角度が変更された
            if (visibility.dGizmoMesh == null)
            {
                //初期IDでなければカウンター減算の必要があるため確認ループ
                if (visibility.dMeshID != -1)
                    for (int i = 0; i < m_meshs.Count; i++)
                    {
                        //ID合致でカウンター減算, break
                        if (visibility.dMeshID == m_meshs[i].id)
                        {
                            var edit = m_meshs[i];
                            edit.counter--;
                            m_meshs[i] = edit;
                            break;
                        }
                    }

                //同じ角度のMeshがないか確認するループ
                for (int i = 0; i < m_meshs.Count; i++)
                {
                    //同じ角度のMeshがある
                    if (visibility.angleVisibility == m_meshs[i].angle)
                    {
                        //visibilityの値変更
                        visibility.dGizmoMesh = m_meshs[i].mesh;
                        visibility.dMeshID = m_meshs[i].id;
                        //カウンター加算, break
                        var edit = m_meshs[i];
                        edit.counter++;
                        m_meshs[i] = edit;
                        break;
                    }
                }

                //Meshがまだnull -> Meshを作る必要がある
                if (visibility.dGizmoMesh == null)
                {
                    //Mesh作成
                    visibility.dGizmoMesh = CreateMesh(visibility.angleVisibility);
                    //メッシュリストに追加
                    m_meshs.Add(new SectorMesh(visibility.dGizmoMesh, visibility.angleVisibility, m_meshID));
                    //ID代入
                    visibility.dMeshID = m_meshID;
                    //IDを次に進める
                    m_meshID++;
                }
            }

            //Mesh描画
            Gizmos.DrawMesh(visibility.dGizmoMesh, position + m_cAdjustUp,
                rotation * Quaternion.AngleAxis(90.0f, Vector3.forward), Vector3.one * visibility.distanceVisibility);
            Gizmos.DrawMesh(visibility.dGizmoMesh, position + m_cAdjustUp,
                rotation * Quaternion.AngleAxis(270.0f, Vector3.forward), Vector3.one * visibility.distanceVisibility);
            Gizmos.DrawMesh(visibility.dGizmoMesh, position + m_cAdjustUp,
                rotation, Vector3.one * visibility.distanceVisibility);

            Gizmos.DrawWireSphere(position, visibility.loseSightDistance);
            Gizmos.matrix = Matrix4x4.Translate(position + m_cAdjustUp + visibility.transform.forward * visibility.distanceVisibility * 0.5f) * Matrix4x4.Rotate(rotation);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.3f, visibility.heightLimitVisibility, visibility.distanceVisibility));
        }

		/// <summary>
		/// [DrawGizmos]
		/// GIzmo描画
		/// 引数1: AIAgentVisibility
		/// 引数2: GizmoType
		/// </summary>
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		static void DrawGizmos(PlayerMaualCollisionAdministrator visibility, GizmoType gizmoType)
		{
			//draw?
			if (!visibility.dIsDrawGizmos) return;

			//削除確認するか経過時間で確認
			if (m_startTime + m_cCheckDestroyTime < EditorApplication.timeSinceStartup)
			{
				m_startTime = EditorApplication.timeSinceStartup;
				//削除ループ
				for (int i = 0; i < m_meshs.Count;)
				{
					//削除可能なら削除
					if (m_meshs[i].isDestroyReady)
						m_meshs.RemoveAt(i);
					else
						i++;
				}
			}

			//Distanceがおかしければ描画しない
			if (visibility.visibilityDistance <= 0.0f)
				return;

			//呼び出しコスト削減
			Transform transform = visibility.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;

			//視界ヒット
			if (visibility.isVisibilityStay)
				Gizmos.color = m_cMeshColorHit;
			//視界ヒットしていない
			else
				Gizmos.color = m_cMeshColorNotHit;

			//visibilityのmeshがnull -> 未設定 or 角度が変更された
			if (visibility.dGizmoMesh == null)
			{
				//初期IDでなければカウンター減算の必要があるため確認ループ
				if (visibility.dMeshID != -1)
					for (int i = 0; i < m_meshs.Count; i++)
					{
						//ID合致でカウンター減算, break
						if (visibility.dMeshID == m_meshs[i].id)
						{
							var edit = m_meshs[i];
							edit.counter--;
							m_meshs[i] = edit;
							break;
						}
					}

				//同じ角度のMeshがないか確認するループ
				for (int i = 0; i < m_meshs.Count; i++)
				{
					//同じ角度のMeshがある
					if (visibility.visibilityAngle == m_meshs[i].angle)
					{
						//visibilityの値変更
						visibility.dGizmoMesh = m_meshs[i].mesh;
						visibility.dMeshID = m_meshs[i].id;
						//カウンター加算, break
						var edit = m_meshs[i];
						edit.counter++;
						m_meshs[i] = edit;
						break;
					}
				}

				//Meshがまだnull -> Meshを作る必要がある
				if (visibility.dGizmoMesh == null)
				{
					//Mesh作成
					visibility.dGizmoMesh = CreateMesh(visibility.visibilityAngle);
					//メッシュリストに追加
					m_meshs.Add(new SectorMesh(visibility.dGizmoMesh, visibility.visibilityAngle, m_meshID));
					//ID代入
					visibility.dMeshID = m_meshID;
					//IDを次に進める
					m_meshID++;
				}
			}

			//Mesh描画
			Gizmos.DrawMesh(visibility.dGizmoMesh, position + m_cAdjustUp,
				rotation * Quaternion.AngleAxis(90.0f, Vector3.forward), Vector3.one * visibility.visibilityDistance);
			Gizmos.DrawMesh(visibility.dGizmoMesh, position + m_cAdjustUp,
				rotation * Quaternion.AngleAxis(270.0f, Vector3.forward), Vector3.one * visibility.visibilityDistance);
			Gizmos.DrawMesh(visibility.dGizmoMesh, position + m_cAdjustUp,
				rotation, Vector3.one * visibility.visibilityDistance);
		}



		/// <summary>
		/// [CreateMesh]
		/// Meshを生成する
		/// 引数1: 角度
		/// </summary>
		static Mesh CreateMesh(float angle)
        {
            //result
            Mesh result = new Mesh();
            //頂点作成
            Vector3[] vertices = CreateVertices(angle);
            //Index作成
            int[] triangleIndexes = new int[m_cTriangleCount * 3];

            //インデックス代入ループ
            for (int arrayIndex = 0, meshIndex = 0;
                arrayIndex < m_cTriangleCount * 3; arrayIndex += 3, meshIndex++)
            {
                triangleIndexes[arrayIndex] = 0;
                triangleIndexes[arrayIndex + 1] = (meshIndex + 1);
                triangleIndexes[arrayIndex + 2] = (meshIndex + 2);
            }

            //頂点, Indexを代入
            result.vertices = vertices;
            result.triangles = triangleIndexes;
            //法線計算
            result.RecalculateNormals();
            //return
            return result;
        }

        /// <summary>
        /// [CreateVertices]
        /// CreateMeshに使用する頂点を生成する
        /// 引数1: 角度
        /// </summary>
        private static Vector3[] CreateVertices(float angle)
        {
            //頂点リスト
            Vector3[] vertices = new Vector3[m_cTriangleCount + 2];

            // 始点
            vertices[0] = Vector3.zero;

            //各Radian値を用意
            float toRadian = angle * Mathf.Deg2Rad;
            float start = -toRadian / 2;
            float add = toRadian / m_cTriangleCount;

            //頂点作成ループ
            for (int i = 1; i < m_cTriangleCount + 2; ++i)
            {
                //Now Radian
                float now = start + (add * (i - 1));
                //頂点作成
                vertices[i] = new Vector3(Mathf.Sin(now), 0.0f, Mathf.Cos(now));
            }

            //return
            return vertices;
        }
    }
}