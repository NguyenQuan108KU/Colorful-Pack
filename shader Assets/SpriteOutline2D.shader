Shader "Custom/SpriteOutline2D"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0,10)) = 2
        _Threshold ("Alpha Threshold", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineSize;
            float _Threshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // If pixel is opaque, draw original sprite pixel
                if (col.a > _Threshold)
                    return col;

                // accumulate neighbor alpha from multiple samples to produce a smooth, thicker outline
                float2 texel = _MainTex_TexelSize.xy * _OutlineSize;

                // first ring (8 directions)
                float alpha =
                    tex2D(_MainTex, i.uv + texel * float2(1,0)).a * 1.0 +
                    tex2D(_MainTex, i.uv + texel * float2(-1,0)).a * 1.0 +
                    tex2D(_MainTex, i.uv + texel * float2(0,1)).a * 1.0 +
                    tex2D(_MainTex, i.uv + texel * float2(0,-1)).a * 1.0 +
                    tex2D(_MainTex, i.uv + texel * float2(1,1)).a * 0.9 +
                    tex2D(_MainTex, i.uv + texel * float2(-1,1)).a * 0.9 +
                    tex2D(_MainTex, i.uv + texel * float2(1,-1)).a * 0.9 +
                    tex2D(_MainTex, i.uv + texel * float2(-1,-1)).a * 0.9;

                // second ring (optional, adds thickness and fills diagonal gaps)
                alpha +=
                    tex2D(_MainTex, i.uv + texel * float2(2,0)).a * 0.45 +
                    tex2D(_MainTex, i.uv + texel * float2(-2,0)).a * 0.45 +
                    tex2D(_MainTex, i.uv + texel * float2(0,2)).a * 0.45 +
                    tex2D(_MainTex, i.uv + texel * float2(0,-2)).a * 0.45;

                // if any neighbors are opaque, draw outline color with alpha based on neighbor coverage
                if (alpha > 0.01)
                {
                    // increase multiplier for a stronger, more visible outline
                    float a = saturate(alpha * 0.35);
                    fixed4 outCol = _OutlineColor;
                    outCol.a *= a;
                    return outCol;
                }

                // else transparent
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}