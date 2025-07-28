Shader "Custom/2DHitGlow"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1, 0, 0, 1)
        _GlowStrength("Glow Strength", Range(0.0, 10.0)) = 2.0
        _HitEffect("Hit Effect", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowStrength;
            float _HitEffect;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float glow = 0.0;

                // Simple glow using alpha threshold sampling
                float alphaThreshold = 0.1;

                // Sample nearby pixels for glow
                float2 offset = float2(_GlowStrength / 100.0, 0);
                glow += tex2D(_MainTex, i.uv + offset).a < alphaThreshold ? 1 : 0;
                glow += tex2D(_MainTex, i.uv - offset).a < alphaThreshold ? 1 : 0;
                offset = float2(0, _GlowStrength / 100.0);
                glow += tex2D(_MainTex, i.uv + offset).a < alphaThreshold ? 1 : 0;
                glow += tex2D(_MainTex, i.uv - offset).a < alphaThreshold ? 1 : 0;

                float glowAmount = saturate(glow * 0.25 * _HitEffect);

                float4 finalColor = col;
                finalColor.rgb += _GlowColor.rgb * glowAmount * _GlowColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}
