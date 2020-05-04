Shader "Custom/OutLine"
{
	//show values to edit in inspector
	Properties
	{
			[HideInInspector]_MainTex("Texture", 2D) = "white" {}
			_OutlineColor("Outline Color", Color) = (0,0,0,1)
			_Depth1("Depth dist 1 multiplier", Range(0,1)) = 0.6
			_Depth1_1("Depth dist 1.4 multiplier", Range(0,1)) = 0.6
			_Depth2("Depth dist 2 multiplier", Range(0,1)) = 0.5
			_Depth2_1("Depth dist 2.2 multiplier", Range(0,1)) = 0.5
			_Depth2_2("Depth dist 2.8 multiplier", Range(0,1)) = 0.5
			_Depth3("Depth dist 3 multiplier", Range(0,1)) = 0.5
			_Depth3_1("Depth dist 3.2 multiplier", Range(0,1)) = 0.5
			_Depth3_2("Depth dist 3.6 multiplier", Range(0,1)) = 0.4

			_Normal1("Normal dist 1 multiplier", Range(0,1)) = 0.8
			_Normal1_1("Normal dist 1.4 multiplier", Range(0,1)) = 0.7
			_Normal2("Normal dist 2 multiplier", Range(0,1)) = 0.5
			_Normal2_1("Normal dist 2.2 multiplier", Range(0,1)) = 0.1
			_Normal2_2("Normal dist 2.8 multiplier", Range(0,1)) = 0.1
			_NormalCutOff("Normal diff Cut-off", Range(0,1)) = 0.04
	}

		SubShader
		{
			// markers that specify that we don't need culling 
			// or comparing/writing to the depth buffer
			Cull Off
			ZWrite Off
			ZTest Always

			Pass
			{
				CGPROGRAM
				//include useful shader functions
				#include "UnityCG.cginc"

				//define vertex and fragment shader
				#pragma vertex vert
				#pragma fragment frag

				//the rendered screen so far
				sampler2D _MainTex;
				//the depth normals texture
				sampler2D _CameraDepthNormalsTexture;
				//texelsize of the depthnormals texture
				float4 _CameraDepthNormalsTexture_TexelSize;

				//variables for customising the effect
				float4 _OutlineColor;
				float _Depth1;
				float _Depth1_1;
				float _Depth2;
				float _Depth2_1;
				float _Depth2_2;
				float _Depth3;
				float _Depth3_1;
				float _Depth3_2;

				float _Normal1;
				float _Normal1_1;
				float _Normal2;
				float _Normal2_1;
				float _Normal2_2;
				float _NormalCutOff;

				//the object data that's put into the vertex shader
				struct appdata 
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				//the data that's used to generate fragments and can be read by the fragment shader
				struct v2f 
				{
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//the vertex shader
				v2f vert(appdata v) 
				{
						v2f o;
						//convert the vertex positions from object space to clip space so they can be rendered
						o.position = UnityObjectToClipPos(v.vertex);
						o.uv = v.uv;
						return o;
				}

				float2 uvOffset(float2 uv, float x, float y)
				{
						return  uv + _CameraDepthNormalsTexture_TexelSize.xy * float2(x, y);
				}

				void CompareNormal1(inout float normalOutline, float3 baseNormal, float2 uv1, float2 uv2, float mulNormal)
				{
					//read neighbor pixel
					float4 neighbor = tex2D(_CameraDepthNormalsTexture, uv1);
					float neighborDepth;
					float3 neighborNormal1;
					DecodeDepthNormal(neighbor, neighborDepth, neighborNormal1);
					neighbor = tex2D(_CameraDepthNormalsTexture, uv2);
					float3 neighborNormal2;
					DecodeDepthNormal(neighbor, neighborDepth, neighborNormal2);


					float3 diff1 = distance(baseNormal, neighborNormal1);
					float3 diff2 = distance(baseNormal, neighborNormal2);
					float3 neighborNormal = diff1 > diff2 ? neighborNormal1 : neighborNormal2;

					float normalDifference = dot(neighborNormal1, neighborNormal2) < dot(baseNormal, neighborNormal) ? max(diff1, diff2) : 0;
					normalDifference = smoothstep(_NormalCutOff, 1, normalDifference * mulNormal);
					normalOutline = normalOutline + normalDifference;
				}
		
				void CompareNormal2(inout float normalOutline, float3 baseNormal, float2 uv1, float2 uv2, float mulNormal) 
				{
					//read neighbor pixel
					float4 neighbor = tex2D(_CameraDepthNormalsTexture, uv1);
					float neighborDepth;
					float3 neighborNormal1;
					DecodeDepthNormal(neighbor, neighborDepth, neighborNormal1);
					neighbor = tex2D(_CameraDepthNormalsTexture, uv2);
					float3 neighborNormal2;
					DecodeDepthNormal(neighbor, neighborDepth, neighborNormal2);
		
		
					float3 diff = distance(neighborNormal1, neighborNormal2);
					float normalDifference = dot(baseNormal, neighborNormal1) > dot(baseNormal, neighborNormal2) ? diff : 0;
					normalDifference = smoothstep(_NormalCutOff, 1, normalDifference * mulNormal);
					normalOutline = normalOutline + normalDifference;
				}
		
				void CompareDepth(inout float depthOutline, float baseDepth, float2 uv, float mulDepth)
				{
					//read neighbor pixel
					float4 neighborDepthnormal = tex2D(_CameraDepthNormalsTexture, uv);
					float3 neighborNormal;
				
					float neighborDepth;
				
					DecodeDepthNormal(neighborDepthnormal, neighborDepth, neighborNormal);
				
					//neighborDepth = neighborDepth * _ProjectionParams.z;
	
					float depthDifference = (baseDepth - neighborDepth) / neighborDepth * mulDepth;
					depthOutline = depthOutline + max(depthDifference, 0);
	
				}

		//the fragment shader
				fixed4 frag(v2f i) : SV_TARGET
				{
					//read depthnormal
					float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv);
		
					//decode depthnormal
					float3 normal;
					float depth;
					DecodeDepthNormal(depthnormal, depth, normal);
		
					//get depth as distance from camera in units 
					//depth = depth * _ProjectionParams.z;
		
					float depthDifference = 0;
					float normalDifference = 0;
		
					// Distance 0-1 Normal
					CompareNormal1(normalDifference, normal, uvOffset(i.uv, 1, 0), uvOffset(i.uv, -1, 0), _Normal1);
					CompareNormal1(normalDifference, normal, uvOffset(i.uv, 0, 1), uvOffset(i.uv, 0, -1), _Normal1);
		
					// Distance 1-1 Normal
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 0), uvOffset(i.uv, -1, 0), _Normal1_1);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 0, 1), uvOffset(i.uv, 0, -1), _Normal1_1);
		
					// Distance 1-1.4 Normal
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 0), uvOffset(i.uv, -1, 1), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 0), uvOffset(i.uv, -1, -1), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 0, 1), uvOffset(i.uv, 1, -1), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 0, 1), uvOffset(i.uv, -1, -1), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 1), uvOffset(i.uv, -1, 0), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, -1), uvOffset(i.uv, -1, 0), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 1), uvOffset(i.uv, 0, -1), _Normal2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, -1, 1), uvOffset(i.uv, 0, -1), _Normal2);
					// Distance 1.4-1.4 Normal
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 1), uvOffset(i.uv, -1, -1), _Normal2_1);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, -1, 1), uvOffset(i.uv, 1, -1), _Normal2_1);
		
					// Distance 1-2 Normal
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 1, 0), uvOffset(i.uv, -2, 0), _Normal2_2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 0, 1), uvOffset(i.uv, 0, -2), _Normal2_2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, -1, 0), uvOffset(i.uv, 2, 0), _Normal2_2);
					CompareNormal2(normalDifference, normal, uvOffset(i.uv, 0, -1), uvOffset(i.uv, 0, 2), _Normal2_2);
		
					// Distance 1
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, 0), _Depth1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 0, 1), _Depth1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 0, -1), _Depth1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, 0), _Depth1);
		
					// Distance SQRT(2)
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, 1), _Depth1_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, -1), _Depth1_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, 1), _Depth1_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, -1), _Depth1_1);
		
					// Distance 2
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, 0), _Depth2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 0, 2), _Depth2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 0, -2), _Depth2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, 0), _Depth2);
		
					// Distance SQRT(4 + 1)
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, 1), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, -1), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, 1), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, -1), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, 2), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, 2), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, -2), _Depth2_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, -2), _Depth2_1);
		
					// Distance SQRT(4 + 4)
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, 2), _Depth2_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, -2), _Depth2_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, 2), _Depth2_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, -2), _Depth2_2);
		
					// Distance 3
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 0, 3), _Depth3);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 3, 0), _Depth3);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 0, -3), _Depth3);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -3, 0), _Depth3);
		
					// Distance SQRT(9 + 1)
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, 3), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, 3), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 1, -3), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -1, -3), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 3, 1), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 3,  -1), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -3, 1), _Depth3_1);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -3,  -1), _Depth3_1);
		
					// Distance SQRT(9 + 4)
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, 3), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, 3), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 2, -3), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -2, -3), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 3, 2), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, 3, -2), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -3, 2), _Depth3_2);
					CompareDepth(depthDifference, depth, uvOffset(i.uv, -3, -2), _Depth3_2);
		
		
					float outline = saturate(max(normalDifference, depthDifference));
					float4 sourceColor = tex2D(_MainTex, i.uv);
					float4 color = lerp(sourceColor, _OutlineColor, outline);
					return color;
				}
				ENDCG
			}
		}
}