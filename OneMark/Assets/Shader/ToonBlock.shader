Shader "Custom/ToonBlock"
{
    Properties
    {
			_Color(":Color", Color) = (1,1,1,1)
			_MainTex(":Albedo (RGB)", 2D) = "white" {}
			_RampTex(":Ramp", 2D) = "white"{}
			_BumpMap(":Bumpmap", 2D) = "bump" {}
			_AreaTex(":AreaTexture", 2D) = "white"{}
			_StageHeight(":StageHeight", Float) = 0
			_StageWidth(":StageWidth", Float) = 0
		}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf ToonRamp

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
				sampler2D _AreaTex;
				sampler2D _RampTex;
				sampler2D _BumpMap;

				float _StageHeight;
				float _StageWidth;

        struct Input
        {
					float3 worldPos;
					float2 uv_MainTex;
					float2 uv_BumpMap;
        };



				fixed4 _Color;

				fixed4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten)
				{
					half d = dot(s.Normal, lightDir)*0.5 + 0.5;
					fixed3 ramp = tex2D(_RampTex, fixed2(d, 0.5)).rgb;
					fixed4 c;
					c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 1.5);
					c.a = s.Alpha;
					return c;
				}

				void surf(Input IN, inout SurfaceOutput o) {
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
					float2 uvArea;
					uvArea.x = 1.0f - IN.worldPos.x / _StageWidth;
					uvArea.y = 1.0f - IN.worldPos.z / _StageHeight;

					fixed4 mask = tex2D(_AreaTex, uvArea);
					o.Albedo = c.rgb * mask.rgb;
					o.Alpha = c.a;
					o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				}
				ENDCG
    }
    FallBack "Diffuse"
}
