Shader "Custom/MarkPointToon"
{
	Properties{
			_Color("Color", Color) = (1,1,1,1)
			_MainTex("Albedo (RGB)", 2D) = "white" {}
			_RampTex("Ramp", 2D) = "white"{}
			_BumpMap("Bumpmap", 2D) = "bump" {}
			_DarkColor("DarkColor", Color) = (1,1,1,1)
			_Height("Height", Float) = 0
			_Gauge("Gauge", Float) = 0
	}
		SubShader{
				Tags { "RenderType" = "Opaque" }
				LOD 200

				CGPROGRAM
				#pragma surface surf ToonRamp vertex:vert
				#pragma target 3.0

				sampler2D _MainTex;
				sampler2D _RampTex;
				sampler2D _BumpMap;

				float _Height;
				float _Gauge;


				fixed4 _DarkColor;

				struct Input {
						float2 uv_MainTex;
						float2 uv_BumpMap;
						float _t;
				};

				fixed4 _Color;

				void vert(inout appdata_full v, out Input o) 
				{
					UNITY_INITIALIZE_OUTPUT(Input, o);
					o._t = v.vertex.y / _Height;
				}

				fixed4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten)
				{
						half d = dot(s.Normal, lightDir)*0.5 + 0.5;
						d *= atten;
						fixed3 ramp = tex2D(_RampTex, fixed2(d, 0.5)).rgb;
						fixed4 c;
						c.rgb = s.Albedo * _LightColor0.rgb * ramp * 2;
						c.a = s.Alpha;
						return c;
				}

				void surf(Input IN, inout SurfaceOutput o) {
						fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
						fixed4 gaugeColor;
						IN._t = step(IN._t, _Gauge);
						gaugeColor.r = saturate(IN._t + _DarkColor.r);
						gaugeColor.g = saturate(IN._t + _DarkColor.g);
						gaugeColor.b = saturate(IN._t + _DarkColor.b);
						gaugeColor.a = 1;
						o.Albedo = c.rgb * gaugeColor;
						o.Alpha = c.a;
						o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				}
				ENDCG
	}
		FallBack "Diffuse"
}
