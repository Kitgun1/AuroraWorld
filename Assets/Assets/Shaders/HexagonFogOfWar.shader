Shader "Custom/HexagonForOfWer"
{

    Properties
    {
        _FogColor ("Fog Color", Color) = (0.1, 0.1, 0.1, 1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        LOD 200

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodynlightmap nodirlightmap novertexlight
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityStandardBRDF.cginc"

            struct hex_edge
            {
                float3 p1;
                float3 p2;
                float4 color;
            };

            StructuredBuffer<hex_edge> _HexEdges;
            int _EdgeCount;
            float _Thickness;
            float _Glossiness;
            float _Metallic;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                SHADOW_COORDS(2)
            };

            float DistanceToLine(float3 p, float3 a, float3 b)
            {
                float3 ab = b - a;
                float3 ap = p - a;
                float t = saturate(dot(ap, ab) / dot(ab, ab));
                return length(ap - ab * t);
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v)
                UNITY_TRANSFER_INSTANCE_ID(v, o)
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(lightDir + viewDir);

                float diff = max(0, dot(i.normal, lightDir));
                float spec = pow(max(0, dot(i.normal, halfDir)), _Glossiness * 128);

                fixed4 col = i.color;
                col.rgb *= diff * _LightColor0.rgb;
                col.rgb += spec * _Metallic * _LightColor0.rgb;

                float minDist = _Thickness;
                float3 lineColor = col.rgb;

                [loop]
                for (int idx = 0; idx < _EdgeCount; idx++)
                {
                    hex_edge edge = _HexEdges[idx];
                    float dist = DistanceToLine(i.worldPos, edge.p1, edge.p2);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        lineColor = edge.color.rgb;
                        if (dist < 0.001) break;
                    }
                }

                if (minDist < _Thickness) col.rgb = lineColor;

                fixed shadow = SHADOW_ATTENUATION(i);
                col.rgb * shadow;

                return lerp(fixed4(0, 0, 0, 1), col, i.color.a);
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode"="ShadowCaster"
            }
            CGPROGRAM
            #pragma vertex vert_shadow
            #pragma fragment frag_shadow
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct appdata_shadow
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f_shadow
            {
                V2F_SHADOW_CASTER;
            };

            v2f_shadow vert_shadow(appdata_shadow v)
            {
                v2f_shadow o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag_shadow(v2f_shadow i):SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    Fallback "Standard"
}