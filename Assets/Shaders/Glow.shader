Shader "BalatroStyle/Glow"
{
    Properties
    {
        _MainTex      ("Sprite Texture", 2D)    = "white" {}
        _Color        ("Tint",           Color) = (0, 1, 0.53, 1)
        _GlowIntensity("Glow Intensity", Range(0, 5)) = 1.5
        _GlowSize     ("Glow Size",      Range(0, 0.05)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType"      = "Transparent"
            "PreviewType"     = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline"  = "UniversalRenderPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                half4  _Color;
                float  _GlowIntensity;
                float  _GlowSize;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                // Sample center alpha
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color * _Color;

                // Simple glow: sample 8 neighbours and accumulate alpha
                float glowAlpha = 0;
                float2 offsets[8] = {
                    float2(-1,-1), float2(0,-1), float2(1,-1),
                    float2(-1, 0),               float2(1, 0),
                    float2(-1, 1), float2(0, 1), float2(1, 1)
                };
                for (int i = 0; i < 8; i++)
                {
                    float2 sampleUV = IN.uv + offsets[i] * _GlowSize;
                    glowAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV).a;
                }
                glowAlpha = saturate(glowAlpha / 8.0);

                // Mix glow colour with sprite colour
                half4 glowColor = _Color * _GlowIntensity;
                glowColor.a = glowAlpha * (1.0 - c.a);

                return c + glowColor * (1.0 - c.a);
            }
            ENDHLSL
        }
    }
}
