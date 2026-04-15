Shader "BalatroStyle/AnimatedBackground"
{
    Properties
    {
        _ColorDark   ("Color Dark",    Color) = (0.059, 0.059, 0.137, 1)
        _ColorMid    ("Color Mid",     Color) = (0.102, 0.102, 0.180, 1)
        _ColorLight  ("Color Light",   Color) = (0.086, 0.129, 0.243, 1)
        _AccentColor ("Accent Color",  Color) = (0.608, 0.349, 0.714, 1)
        _AccentStr   ("Accent Strength",   Range(0, 0.30)) = 0.08
        _SwirlSpeed  ("Swirl Speed",       Range(0, 1.0))  = 0.12
        _SwirlScale  ("Swirl Scale",       Range(0.5, 8))  = 2.5
        _FlowAmount  ("Flow Amount",       Range(0, 0.25)) = 0.05
        _PulseSpeed  ("Pulse Speed",       Range(0, 2.0))  = 0.40
        _PulseStr    ("Pulse Strength",    Range(0, 0.50)) = 0.12
        _Vignette    ("Vignette",          Range(0, 2.0))  = 0.60
        _VignetteMix ("Vignette Mix",      Range(0, 1.0))  = 0.60
    }

    SubShader
    {
        Tags
        {
            "Queue"          = "Background"
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
            "PreviewType"    = "Plane"
        }
        LOD 100
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4  _ColorDark;
                half4  _ColorMid;
                half4  _ColorLight;
                half4  _AccentColor;
                float  _AccentStr;
                float  _SwirlSpeed;
                float  _SwirlScale;
                float  _FlowAmount;
                float  _PulseSpeed;
                float  _PulseStr;
                float  _Vignette;
                float  _VignetteMix;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // Cheap layered-sine flow field — no texture lookups, scales to any resolution.
            float Swirl(float2 uv, float t)
            {
                float a = sin(uv.x * _SwirlScale + t) * 0.5 + 0.5;
                float b = sin((uv.y + uv.x * 0.6) * _SwirlScale * 0.8 - t * 0.7) * 0.5 + 0.5;
                float c = sin((uv.x - uv.y) * _SwirlScale * 1.3 + t * 0.9) * 0.5 + 0.5;
                return saturate((a + b + c) / 3.0);
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float t = _Time.y * _SwirlSpeed;

                // Distort sample position so the gradient breathes and drifts.
                float2 flow = float2(
                    sin(IN.uv.y * _SwirlScale + t) * _FlowAmount,
                    cos(IN.uv.x * _SwirlScale - t * 0.8) * _FlowAmount
                );
                float n = Swirl(IN.uv + flow, t);

                // Blend the three palette tones along the noise field.
                half3 col = lerp(_ColorDark.rgb,  _ColorMid.rgb,   smoothstep(0.00, 0.60, n));
                col       = lerp(col,             _ColorLight.rgb, smoothstep(0.55, 1.00, n));

                // Slow accent pulse — barely visible, hints at life.
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                col += _AccentColor.rgb * _AccentStr * pulse * smoothstep(0.70, 1.00, n);

                // Soft radial vignette centered on the quad.
                float2 d = IN.uv - 0.5;
                float vig = 1.0 - saturate(dot(d, d) * _Vignette * 4.0);
                col *= lerp(1.0, vig, _VignetteMix);

                // Overall breathing multiplier.
                col *= 1.0 + (pulse - 0.5) * _PulseStr;

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
