Shader "Hidden/Facility Outline"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Facility Outline Mask"

            Cull Back
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex MaskVertex
            #pragma fragment MaskFragment
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings MaskVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 MaskFragment(Varyings input) : SV_Target
            {
                return half4(1.0h, 1.0h, 1.0h, 1.0h);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Facility Outline Composite"

            Cull Off
            ZWrite Off
            ZTest Always

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment CompositeFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D_X(_FacilityOutlineMaskTexture);

            half4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineSoftness;

            float SampleMask(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(
                    _FacilityOutlineMaskTexture,
                    sampler_LinearClamp,
                    uv).r;
            }

            float SampleOutlineRing(float2 uv, float2 radius)
            {
                const float diagonal = 0.70710678;
                float mask = 0.0;

                mask = max(mask, SampleMask(uv + float2( radius.x, 0.0)));
                mask = max(mask, SampleMask(uv + float2(-radius.x, 0.0)));
                mask = max(mask, SampleMask(uv + float2(0.0,  radius.y)));
                mask = max(mask, SampleMask(uv + float2(0.0, -radius.y)));
                mask = max(mask, SampleMask(uv + radius * float2( diagonal,  diagonal)));
                mask = max(mask, SampleMask(uv + radius * float2(-diagonal,  diagonal)));
                mask = max(mask, SampleMask(uv + radius * float2( diagonal, -diagonal)));
                mask = max(mask, SampleMask(uv + radius * float2(-diagonal, -diagonal)));

                return mask;
            }

            half4 CompositeFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                half4 source = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv);

                float centerMask = SampleMask(uv);
                float2 pixelSize = rcp(_ScreenParams.xy);
                float innerWidth = lerp(_OutlineWidth, 1.0, _OutlineSoftness);

                float innerMask = SampleOutlineRing(
                    uv,
                    pixelSize * innerWidth);
                float outerMask = SampleOutlineRing(
                    uv,
                    pixelSize * _OutlineWidth);

                float innerEdge = saturate(innerMask - centerMask);
                float outerEdge = saturate(outerMask - centerMask);
                float outerAlpha = lerp(1.0, 0.25, _OutlineSoftness);
                float outline = max(innerEdge, outerEdge * outerAlpha);
                float blend = outline * _OutlineColor.a;

                source.rgb = lerp(source.rgb, _OutlineColor.rgb, blend);
                return source;
            }
            ENDHLSL
        }
    }
}
