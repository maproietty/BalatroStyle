Shader "BalatroStyle/CRT"
{
    Properties
    {
        _MainTex       ("Texture", 2D)      = "white" {}
        _ScanlineCount ("Scanline Count",   Range(100, 800))  = 240
        _ScanlineAlpha ("Scanline Alpha",   Range(0, 0.5))    = 0.18
        _Curvature     ("Curvature",        Range(0, 0.5))    = 0.08
        _VignetteStr   ("Vignette Strength",Range(0, 2))      = 0.55
        _ColorBleed    ("Color Bleed",      Range(0, 0.015))  = 0.003
        _Brightness    ("Brightness",       Range(0.5, 1.5))  = 1.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100
        ZWrite Off Cull Off

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
                float  _ScanlineCount;
                float  _ScanlineAlpha;
                float  _Curvature;
                float  _VignetteStr;
                float  _ColorBleed;
                float  _Brightness;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            // Barrel distortion
            float2 CurveUV(float2 uv)
            {
                uv = uv * 2.0 - 1.0;
                float2 offset = abs(uv.yx) / float2(6.0 / _Curvature, 6.0 / _Curvature);
                uv = uv + uv * offset * offset;
                return uv * 0.5 + 0.5;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = CurveUV(IN.uv);

                // Discard outside curved screen
                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    return float4(0, 0, 0, 1);

                // Chromatic color bleed (RGB fringe)
                float4 col;
                col.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(_ColorBleed, 0)).r;
                col.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).g;
                col.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(_ColorBleed, 0)).b;
                col.a = 1.0;

                // Scanlines
                float scanline = sin(uv.y * _ScanlineCount * 3.14159) * 0.5 + 0.5;
                col.rgb *= 1.0 - _ScanlineAlpha * (1.0 - scanline);

                // Vignette
                float2 vigUV = uv * (1.0 - uv.yx);
                float vig = vigUV.x * vigUV.y * 15.0;
                vig = pow(saturate(vig), _VignetteStr);
                col.rgb *= vig;

                col.rgb *= _Brightness;
                return col;
            }
            ENDHLSL
        }
    }
}
