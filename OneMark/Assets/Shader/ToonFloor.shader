Shader "Custom/ToonFloor"
{
	Properties{
			_Color("Color", Color) = (1,1,1,1)
			_MainTex("Albedo (RGB)", 2D) = "white" {}
			_RampTex("Ramp", 2D) = "white"{}
			_BumpMap("Bumpmap", 2D) = "bump" {}
			_AreaTex("AreaTexture", 2D) = "white"{}
			_SafetyAreaTex("SafetyAreaTexture", 2D) = "white"{}
	}
		
	SubShader
	{
	
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf ToonRamp
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _AreaTex;
		sampler2D _SafetyAreaTex;
		sampler2D _RampTex;
		sampler2D _BumpMap;

		struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 worldPos;
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
				fixed4 c = tex2D(_MainTex, float2(IN.worldPos.x / 10, IN.worldPos.z / 10)) * _Color;
				fixed4 dangerMask = tex2D(_AreaTex, IN.uv_MainTex);
				fixed4 mask = tex2D(_AreaTex, IN.uv_MainTex);
				fixed4 safetyMask = tex2D(_SafetyAreaTex, IN.uv_MainTex);
				mask.r = mask.g = mask.b = max(step(mask.b, 0), mask.r);
				c.rgb = c.rgb * mask.rgb;
				c.r = lerp(c.r, dangerMask.r, mask.a * (1 - safetyMask.a));
				c.g = lerp(c.g, dangerMask.g, mask.a * (1 - safetyMask.a));
				c.b = lerp(c.b, dangerMask.b, mask.a * (1 - safetyMask.a));
				o.Albedo = c.rgb;
				o.Alpha = c.a;
				o.Normal = UnpackNormal(tex2D(_BumpMap, float2(IN.worldPos.x / 10, IN.worldPos.z / 10)));
		} 
		ENDCG
	}
	FallBack "Diffuse"
}
