Shader "Custom/SpriteOutlineOutside"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0,20)) = 0

    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineSize;
            float4 _MainTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                if (col.a > 0)
                    return col;

                float alpha = 0;
                float2 px = _MainTex_TexelSize.xy;

                // 🔥 loop cố định (an toàn)
                for (int x = -25; x <= 25; x++)
                {
                    for (int y = -25; y <= 25; y++)
                    {
                        float dist = sqrt(x*x + y*y);
                        if (dist > _OutlineSize) continue;

                        float2 offset = float2(x, y) * px;
                        alpha = max(alpha, tex2D(_MainTex, i.uv + offset).a);
                    }
                }

                if (alpha > 0)
{
    fixed4 o = _OutlineColor;
    o.a = 4;   // luôn full alpha
    return o;
}


                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
