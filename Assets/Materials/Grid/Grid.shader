Shader "Custom/GlowingGrid"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _GridSpacing ("Grid Spacing", Float) = 1
        _LineWidth ("Line Width", Float) = 0.05
        _EmissionStrength ("Emission Strength", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };
            
            fixed4 _GridColor;
            fixed4 _BackgroundColor;
            float _GridSpacing;
            float _LineWidth;
            float _EmissionStrength;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 absNormal = abs(i.normal);
                float2 gridUV;
                
                // 根據法線選擇對應的投影平面
                if (absNormal.y > absNormal.x && absNormal.y > absNormal.z)
                    gridUV = frac(i.worldPos.xz / _GridSpacing); // XZ 平面
                else if (absNormal.x > absNormal.y && absNormal.x > absNormal.z)
                    gridUV = frac(i.worldPos.yz / _GridSpacing); // YZ 平面
                else
                    gridUV = frac(i.worldPos.xy / _GridSpacing); // XY 平面
                
                float lineX = min(gridUV.x, 1.0 - gridUV.x);
                float lineY = min(gridUV.y, 1.0 - gridUV.y);
                float gridLine = min(lineX, lineY) * 2.0;
                
                float gridMask = smoothstep(_LineWidth, 0.0, gridLine);
                
                fixed4 color = lerp(_GridColor, _BackgroundColor, gridMask);
                
                // HDR Emission 修正
                color.rgb += _GridColor.rgb * (1.0 - gridMask) * _EmissionStrength;
                color.rgb = saturate(color.rgb);
                color.rgb *= _EmissionStrength; // 這行確保發光強度顯示
                
                return color;
            }
            ENDCG
        }
    }
}

