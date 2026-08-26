Shader "UI/OutlineOnly"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        
        // 두께 제한 15
        _OutlineWidth ("Outline Width", Range(0, 15)) = 1

        // UI 캔버스 렌더링에 필수적인 프로퍼티들
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR; 
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                // worldPosition 연산 제거됨 (클리핑을 안 쓰기 때문)
                OUT.vertex = UnityObjectToClipPos(v.vertex); 

                OUT.texcoord = v.texcoord;
                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 원본 텍스처의 알파(투명도)값만 가져옵니다.
                half originalAlpha = tex2D(_MainTex, IN.texcoord).a;
                
                // 외곽선 두께만큼 상하좌우 픽셀을 확인하여 테두리 영역을 찾습니다.
                float2 up = float2(0, _MainTex_TexelSize.y) * _OutlineWidth;
                float2 down = float2(0, -_MainTex_TexelSize.y) * _OutlineWidth;
                float2 right = float2(_MainTex_TexelSize.x, 0) * _OutlineWidth;
                float2 left = float2(-_MainTex_TexelSize.x, 0) * _OutlineWidth;

                float outlineAlpha = originalAlpha;
                outlineAlpha = max(outlineAlpha, tex2D(_MainTex, IN.texcoord + up).a);
                outlineAlpha = max(outlineAlpha, tex2D(_MainTex, IN.texcoord + down).a);
                outlineAlpha = max(outlineAlpha, tex2D(_MainTex, IN.texcoord + right).a);
                outlineAlpha = max(outlineAlpha, tex2D(_MainTex, IN.texcoord + left).a);

                // 원본 이미지가 있는 '안쪽'은 투명하게 파내고, 순수하게 바깥 '테두리'만 남깁니다.
                fixed4 finalColor = _OutlineColor * IN.color;
                finalColor.a *= (outlineAlpha - originalAlpha);
                
                // 마스크 클리핑 코드(UnityGet2DClipping) 완전히 제거됨!

                return finalColor;
            }
            ENDCG
        }
    }
}