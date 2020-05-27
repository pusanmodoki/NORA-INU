Shader "Unlit/AreaBorderLine"
{
    Properties
    {
				_Color("Color", Color) = (1, 1, 1, 1)
				_Color2("Color2", Color) = (1, 1, 1, 1)
				_Height("Height", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
						Cull Off
						Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
						  	float4 color : COLOR0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

						
						float _Height;

						float4 _Color;
						float4 _Color2;

						float random(fixed2 p) {
							return frac(sin(dot(p, fixed2(12.9898, 78.233))) * 43758.5453);
						}
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
								o.color = _Color * ((_Height - v.vertex.y) / _Height) + _Color2 * v.vertex.y / _Height;
								o.color.a = ((_Height - v.vertex.y) / _Height) - (v.vertex.y / _Height * sin(_Time * 30 + v.vertex.x *- v.vertex.z) * 0.5 + 0.5) ;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
								fixed4 col = i.color;
								col.a = max(0, i.color.a);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
