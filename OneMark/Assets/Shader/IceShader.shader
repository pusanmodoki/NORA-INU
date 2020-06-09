﻿Shader "Custom/NewSurfaceShader"
{
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		struct Input {
			float3 worldNormal;
			float3 viewDir;
		};

		fixed4 _Color;
		
		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
			float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
			o.Alpha = alpha * 1.5f;
		}
		ENDCG
    }
    FallBack "Diffuse"
}
