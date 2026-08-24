Shader "UI/Background Blur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _TintColor ("Tint", Color) = (0.08, 0.10, 0.14, 0.25)
        _BlurRadius ("Blur Radius", Range(0, 12)) = 4
        _Opacity ("Opacity", Range(0, 1)) = 0.9

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UIBackgroundBlur"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _TintColor;
                float _BlurRadius;
                half _Opacity;
                float4 _ClipRect;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                float4 screenPosition : TEXCOORD1;
                float2 positionOS : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.screenPosition = ComputeScreenPos(output.positionCS);
                output.positionOS = input.positionOS.xy;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half spriteAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
                half alpha = spriteAlpha * input.color.a * _Opacity;

                #ifdef UNITY_UI_CLIP_RECT
                    float2 inside = step(_ClipRect.xy, input.positionOS) * step(input.positionOS, _ClipRect.zw);
                    alpha *= inside.x * inside.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(alpha - 0.001h);
                #endif

                float2 screenUV = input.screenPosition.xy / input.screenPosition.w;
                float2 offset = _CameraOpaqueTexture_TexelSize.xy * _BlurRadius;

                half3 blurred = SampleSceneColor(screenUV) * 0.20h;
                blurred += SampleSceneColor(screenUV + float2( offset.x, 0.0)) * 0.12h;
                blurred += SampleSceneColor(screenUV + float2(-offset.x, 0.0)) * 0.12h;
                blurred += SampleSceneColor(screenUV + float2(0.0,  offset.y)) * 0.12h;
                blurred += SampleSceneColor(screenUV + float2(0.0, -offset.y)) * 0.12h;
                blurred += SampleSceneColor(screenUV + float2( offset.x,  offset.y)) * 0.08h;
                blurred += SampleSceneColor(screenUV + float2(-offset.x,  offset.y)) * 0.08h;
                blurred += SampleSceneColor(screenUV + float2( offset.x, -offset.y)) * 0.08h;
                blurred += SampleSceneColor(screenUV + float2(-offset.x, -offset.y)) * 0.08h;

                half3 tinted = lerp(blurred, _TintColor.rgb, _TintColor.a);
                return half4(tinted, alpha);
            }
            ENDHLSL
        }
    }
}
