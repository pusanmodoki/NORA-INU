Shader "Custom/IceDenchu"
{
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_DissolveColor("DissolveColor", Color) = (1, 1, 1, 1)
		_DissolveHeight("DissolveHeight", Float) = 0
		_AlphaCorrection("Alpha",Float ) = 1.5
		_Height("Height", Float) = 0
		_Gauge("Gauge", Float) = 0
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		struct Input {
			float3 worldNormal;
			float3 viewDir;
			float height;
		};

		fixed4 _Color;
		fixed4 _DissolveColor;

		float _DissolveHeight;

		float _AlphaCorrection;

		float _Height;
		float _Gauge;

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.height = v.vertex.y / _Height;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			o.Albedo = _Color;
			float alpha = step(IN.height, 1 - _Gauge);
			alpha *= 1 - (abs(dot(IN.viewDir, IN.worldNormal)));

			o.Alpha = alpha * _AlphaCorrection;
			alpha = step(1 - _Gauge, IN.height) * step(IN.height, 1 - _Gauge - _DissolveHeight);
			o.Alpha = saturate(o.Alpha + alpha);
		}
		ENDCG
    }
    FallBack "Diffuse"
}
