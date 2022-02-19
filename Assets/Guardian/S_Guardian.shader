Shader "MUCO/S_Guardian"
{
    Properties
    {
        u_Color("Color", Color) = (0.9, 0.9, 0.9)
        u_Zoom("Zoom", float) = 8.0
        u_Radius("Radius", float) = 0.1
    }
    SubShader {
        Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            half3 ObjectScale() {
                return half3(length(unity_ObjectToWorld._m00_m10_m20), length(unity_ObjectToWorld._m01_m11_m21), length(unity_ObjectToWorld._m02_m12_m22));
            }

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragmentInput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fragmentInput vert(vertexInput v)
            {
                fragmentInput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 movingTiles(float2 _st, float _zoom)
            {
                _st *= _zoom;
                float time = 0.15f;
                if (frac(time) > 0.5) {
                    if (frac(_st.y * 0.5) > 0.5) {
                        _st.x += frac(time) * 2.0;  
                    }
                    else {
                        _st.x -= frac(time) * 2.0;
                    }
                }
                else {
                    if (frac(_st.x * 0.5) > 0.5) {
                        _st.y += frac(time) * 2.0;
                    }
                    else {
                        _st.y -= frac(time) * 2.0;
                    }
                }
                return frac(_st);
            }

            float circle(float2 _st, float _radius)
            {
                float2 pos = float2(0.5, 0.5) - _st;
                return smoothstep(1.0 - _radius, 1.0 - _radius + _radius * 0.2, 1. - dot(pos, pos) * 3.14);
            }

            float u_Zoom;
            float u_Radius;
            fixed4 frag(fragmentInput i) : SV_Target
            {
                float2 uv = i.uv;
                half3 objectScale = ObjectScale();
                uv.x *= objectScale.x;
                uv.y *= objectScale.y;
                float2 st = movingTiles(uv, u_Zoom);
                fixed4 col = fixed4(1.0f, 1.0f, 1.0f, circle(st, u_Radius));
                clip(col.a - 0.7);
                return col;
            }
            ENDCG
        }
    }
}