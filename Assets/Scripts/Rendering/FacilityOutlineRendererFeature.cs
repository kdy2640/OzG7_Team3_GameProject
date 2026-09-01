// 여러 ShaderTagId를 보관하기 위해 사용합니다.
using System.Collections.Generic;

// Material, Color, CameraType 같은 Unity 기본 타입을 사용합니다.
using UnityEngine;

// GraphicsFormat.R8_UNorm 같은 GPU 텍스처 포맷을 사용합니다.
using UnityEngine.Experimental.Rendering;

// DrawingSettings, FilteringSettings 같은 렌더링 타입을 사용합니다.
using UnityEngine.Rendering;

// RenderGraph와 TextureHandle 같은 RenderGraph 타입을 사용합니다.
using UnityEngine.Rendering.RenderGraphModule;

// RenderGraph의 AddBlitPass 확장 메서드를 사용합니다.
using UnityEngine.Rendering.RenderGraphModule.Util;

// ScriptableRendererFeature와 URP 전용 프레임 데이터를 사용합니다.
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP Renderer에 시설 선택 외곽선 렌더링을 추가하는 기능입니다.
///
/// ScriptableRendererFeature는 직접 그림을 그리지 않습니다.
/// 대신 언제 어떤 ScriptableRenderPass를 실행할지 URP에 등록합니다.
/// 실제 마스크 생성과 화면 합성은 아래의 FacilityOutlinePass가 담당합니다.
/// </summary>
public sealed class FacilityOutlineRendererFeature : ScriptableRendererFeature
{
    // 1을 왼쪽으로 7칸 이동하여 7번 Rendering Layer 비트를 만듭니다.
    // FacilityModelView와 Mask Pass가 같은 비트를 사용해야 선택 모델만 찾을 수 있습니다.
    internal const uint SelectedRenderingLayerMask = 1u << 7;

    // Mask Pass와 Composite Pass를 모두 가지고 있는 전용 머티리얼입니다.
    // PC_Renderer와 Mobile_Renderer의 Facility Outline 항목에서 연결됩니다.
    [SerializeField] private Material outlineMaterial;

    // Inspector에서 1보다 큰 HDR 색상값도 입력할 수 있게 합니다.
    [ColorUsage(true, true)]

    // 최종 화면에 합성할 외곽선 색상입니다.
    [SerializeField] private Color outlineColor =
        new Color(1.4f, 0.65f, 0.1f, 1f);

    // 화면 픽셀 기준 외곽선 두께입니다.
    [SerializeField, Range(1f, 6f)] private float outlineWidth = 3f;

    // 0이면 비교적 선명하고, 1에 가까울수록 바깥 테두리가 부드러워집니다.
    [SerializeField, Range(0f, 1f)] private float outlineSoftness = 0.35f;

    // 현재 선택된 시설이 있는지 나타냅니다.
    // false일 때는 외곽선 렌더 패스를 등록하지 않아 GPU 작업을 생략합니다.
    private static bool isSelectionActive;

    // 실제 마스크 생성과 화면 합성을 기록할 렌더 패스 인스턴스입니다.
    private FacilityOutlinePass outlinePass;

    /// <summary>
    /// FacilityCollection이 시설 선택 또는 선택 해제 시 호출합니다.
    /// </summary>
    internal static void SetSelectionActive(bool isActive)
    {
        // Renderer Feature가 이번 프레임에 실행될 수 있는지 저장합니다.
        isSelectionActive = isActive;
    }

    /// <summary>
    /// Unity가 Renderer Feature를 생성하거나 다시 직렬화할 때 호출합니다.
    /// 여기서는 실제로 사용할 ScriptableRenderPass를 준비합니다.
    /// </summary>
    public override void Create()
    {
        // 실제 렌더링 작업을 담당할 패스를 한 번 생성합니다.
        outlinePass = new FacilityOutlinePass
        {
            // 투명 오브젝트 렌더링 이후, Post Processing 이전에 실행합니다.
            // 이 시점이면 0레벨 Transparent 시설도 원본 화면에 그려진 상태입니다.
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    /// <summary>
    /// URP가 카메라를 렌더링할 때마다 호출합니다.
    /// 조건을 검사한 뒤 이번 카메라에 FacilityOutlinePass를 등록합니다.
    /// </summary>
    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        // 선택된 시설이 없거나 머티리얼 연결이 없으면 아무 작업도 하지 않습니다.
        if (!isSelectionActive || outlineMaterial == null)
            return;

        // Game Camera가 아니면 Scene View, Preview 같은 편집기 카메라일 수 있습니다.
        // Base Camera만 허용하여 Overlay Camera에서 효과가 중복 실행되는 것도 막습니다.
        if (renderingData.cameraData.cameraType != CameraType.Game
            || renderingData.cameraData.renderType != CameraRenderType.Base)
        {
            // 현재 카메라에는 외곽선 패스를 등록하지 않습니다.
            return;
        }

        // Inspector에 저장된 현재 외곽선 설정을 실제 패스에 전달합니다.
        outlinePass.Setup(
            outlineMaterial,
            outlineColor,
            outlineWidth,
            outlineSoftness);

        // URP의 현재 카메라 렌더링 순서에 외곽선 패스를 추가합니다.
        renderer.EnqueuePass(outlinePass);
    }

    /// <summary>
    /// 선택 모델 마스크 생성과 최종 화면 합성을 담당하는 실제 렌더 패스입니다.
    /// 하나의 ScriptableRenderPass 안에서 RenderGraph 패스 세 개를 순서대로 기록합니다.
    ///
    /// 1. 선택 모델 마스크 생성
    /// 2. 현재 카메라 색상 복사
    /// 3. 카메라 색상과 마스크 합성
    /// </summary>
    private sealed class FacilityOutlinePass : ScriptableRenderPass
    {
        // 셰이더의 _FacilityOutlineMaskTexture 프로퍼티 ID를 한 번만 계산해 저장합니다.
        private static readonly int MaskTextureId =
            Shader.PropertyToID("_FacilityOutlineMaskTexture");

        // 셰이더의 _OutlineColor 프로퍼티 ID입니다.
        private static readonly int OutlineColorId =
            Shader.PropertyToID("_OutlineColor");

        // 셰이더의 _OutlineWidth 프로퍼티 ID입니다.
        private static readonly int OutlineWidthId =
            Shader.PropertyToID("_OutlineWidth");

        // 셰이더의 _OutlineSoftness 프로퍼티 ID입니다.
        private static readonly int OutlineSoftnessId =
            Shader.PropertyToID("_OutlineSoftness");

        // 원본 시설 셰이더에서 어떤 LightMode Pass를 찾을지 지정합니다.
        // 시설마다 셰이더 종류가 달라도 아래 태그 중 하나가 있으면 마스크 대상이 됩니다.
        private readonly List<ShaderTagId> shaderTagIds = new()
        {
            // URP Forward 렌더링에서 사용하는 대표 패스입니다.
            new ShaderTagId("UniversalForward"),

            // Deferred에서도 Forward로만 그려지는 머티리얼이 사용하는 패스입니다.
            new ShaderTagId("UniversalForwardOnly"),

            // URP Deferred 렌더링의 GBuffer 패스입니다.
            new ShaderTagId("UniversalGBuffer"),

            // Unlit 또는 별도 LightMode가 없는 단순 셰이더용 패스입니다.
            new ShaderTagId("SRPDefaultUnlit")
        };

        // 이번 프레임에 Mask/Composite Pass에서 사용할 머티리얼입니다.
        private Material material;

        // 이번 프레임에 사용할 외곽선 색상입니다.
        private Color color;

        // 이번 프레임에 사용할 픽셀 단위 두께입니다.
        private float width;

        // 이번 프레임에 사용할 외곽선 부드러움입니다.
        private float softness;

        /// <summary>
        /// 패스가 생성될 때 카메라 색상 중간 텍스처가 필요하다고 URP에 알립니다.
        /// </summary>
        public FacilityOutlinePass()
        {
            // Composite Pass는 카메라 색상을 읽은 뒤 다시 써야 합니다.
            // Back Buffer를 직접 읽을 수 없으므로 URP가 중간 Color Texture를 준비하게 합니다.
            requiresIntermediateTexture = true;
        }

        /// <summary>
        /// Renderer Feature의 Inspector 설정을 이번 프레임 패스에 복사합니다.
        /// </summary>
        internal void Setup(
            Material targetMaterial,
            Color targetColor,
            float targetWidth,
            float targetSoftness)
        {
            // Mask와 Composite에서 사용할 머티리얼을 저장합니다.
            material = targetMaterial;

            // 최종 합성 색상을 저장합니다.
            color = targetColor;

            // 외곽선 두께를 저장합니다.
            width = targetWidth;

            // 외곽선 부드러움을 저장합니다.
            softness = targetSoftness;
        }

        /// <summary>
        /// URP가 이번 카메라 프레임의 RenderGraph를 만들 때 호출합니다.
        /// 여기서 필요한 텍스처를 만들고 GPU 작업의 순서와 의존성을 기록합니다.
        /// </summary>
        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            // 현재 카메라의 Color/Depth Texture를 얻기 위한 프레임 데이터입니다.
            UniversalResourceData resourceData =
                frameData.Get<UniversalResourceData>();

            // 현재 카메라의 해상도, 포맷 같은 정보를 얻기 위한 프레임 데이터입니다.
            UniversalCameraData cameraData =
                frameData.Get<UniversalCameraData>();

            // 선택 모델의 실루엣을 저장할 흑백 마스크 텍스처를 만듭니다.
            TextureHandle maskTexture = CreateMaskTexture(
                renderGraph,
                cameraData);

            // 선택 모델을 maskTexture에 흰색으로 그리는 패스를 기록합니다.
            RecordMaskPass(
                renderGraph,
                frameData,
                resourceData,
                maskTexture);

            // 현재 카메라 화면을 안전하게 읽기 위한 복사본 텍스처를 만듭니다.
            TextureHandle sourceTexture = CreateSourceTexture(
                renderGraph,
                cameraData);

            // 현재 카메라 색상을 sourceTexture로 복사하는 Blit Pass를 기록합니다.
            renderGraph.AddBlitPass(
                resourceData.activeColorTexture,
                sourceTexture,
                Vector2.one,
                Vector2.zero,
                passName: "Facility Outline Copy Color");

            // 복사한 카메라 화면과 마스크를 읽어 최종 외곽선을 합성합니다.
            RecordCompositePass(
                renderGraph,
                resourceData,
                sourceTexture,
                maskTexture);
        }

        /// <summary>
        /// 선택 모델의 실루엣만 저장할 한 채널 마스크 텍스처를 만듭니다.
        /// </summary>
        private static TextureHandle CreateMaskTexture(
            RenderGraph renderGraph,
            UniversalCameraData cameraData)
        {
            // 카메라와 같은 크기, XR 형태, 동적 해상도 설정을 가져옵니다.
            RenderTextureDescriptor descriptor =
                cameraData.cameraTargetDescriptor;

            // 마스크 텍스처 자체에는 별도 깊이 버퍼를 만들지 않습니다.
            descriptor.depthStencilFormat = GraphicsFormat.None;

            // 현재 프로젝트가 MSAA 1배이므로 마스크도 1샘플로 만듭니다.
            descriptor.msaaSamples = 1;

            // 흰색과 검은색만 필요하므로 8비트 단일 채널 포맷을 사용합니다.
            descriptor.graphicsFormat = GraphicsFormat.R8_UNorm;

            // RenderGraph가 수명을 관리하는 임시 마스크 텍스처를 생성합니다.
            return UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                descriptor,
                "_FacilityOutlineMaskTexture",
                true,
                FilterMode.Point);
        }

        /// <summary>
        /// Composite Pass가 읽을 카메라 색상 복사본을 만듭니다.
        /// </summary>
        private static TextureHandle CreateSourceTexture(
            RenderGraph renderGraph,
            UniversalCameraData cameraData)
        {
            // 카메라와 같은 크기와 색상 포맷을 가져옵니다.
            RenderTextureDescriptor descriptor =
                cameraData.cameraTargetDescriptor;

            // 색상 복사본에는 깊이 버퍼가 필요하지 않습니다.
            descriptor.depthStencilFormat = GraphicsFormat.None;

            // 색상 복사본도 단일 샘플 텍스처로 만듭니다.
            descriptor.msaaSamples = 1;

            // RenderGraph가 수명을 관리하는 임시 카메라 색상 텍스처를 생성합니다.
            return UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                descriptor,
                "_FacilityOutlineSourceTexture",
                false,
                FilterMode.Bilinear);
        }

        /// <summary>
        /// Facility Selected Rendering Layer를 가진 모델만 흰색 마스크에 그립니다.
        /// 카메라 Depth Texture를 읽기 때문에 가려진 부분은 마스크에 들어가지 않습니다.
        /// </summary>
        private void RecordMaskPass(
            RenderGraph renderGraph,
            ContextContainer frameData,
            UniversalResourceData resourceData,
            TextureHandle maskTexture)
        {
            // RenderGraph에 일반 래스터 드로우 패스를 하나 추가합니다.
            using IRasterRenderGraphBuilder builder =
                renderGraph.AddRasterRenderPass<MaskPassData>(
                    "Facility Outline Mask",
                    out MaskPassData passData);

            // 현재 카메라 Culling 결과를 얻습니다.
            UniversalRenderingData renderingData =
                frameData.Get<UniversalRenderingData>();

            // 카메라 정렬 기준을 얻습니다.
            UniversalCameraData cameraData =
                frameData.Get<UniversalCameraData>();

            // DrawingSettings 생성에 필요한 조명 프레임 데이터를 얻습니다.
            UniversalLightData lightData =
                frameData.Get<UniversalLightData>();

            // 어떤 Renderer를 마스크에 그릴지 필터를 만듭니다.
            FilteringSettings filteringSettings = new(
                // Opaque와 0레벨 Transparent 시설을 모두 허용합니다.
                RenderQueueRange.all,

                // 일반 GameObject Layer는 제한하지 않습니다.
                -1,

                // Facility Selected Rendering Layer 비트가 있는 Renderer만 허용합니다.
                SelectedRenderingLayerMask);

            // 어떤 셰이더 패스와 정렬 방식으로 Renderer를 그릴지 설정합니다.
            DrawingSettings drawingSettings =
                RenderingUtils.CreateDrawingSettings(
                    shaderTagIds,
                    renderingData,
                    cameraData,
                    lightData,
                    cameraData.defaultOpaqueSortFlags);

            // 원본 시설 머티리얼 대신 외곽선 전용 머티리얼로 그립니다.
            // 이 변경은 마스크 패스에만 적용되며 실제 Renderer 머티리얼은 바뀌지 않습니다.
            drawingSettings.overrideMaterial = material;

            // FacilityOutline.shader의 0번 Pass인 Mask Pass를 사용합니다.
            drawingSettings.overrideMaterialPassIndex = 0;

            // Culling 결과, 드로우 설정, 필터를 하나로 묶습니다.
            RendererListParams rendererListParams = new(
                renderingData.cullResults,
                drawingSettings,
                filteringSettings);

            // 조건을 통과한 Renderer를 GPU가 그릴 수 있는 RendererList로 만듭니다.
            passData.rendererList =
                renderGraph.CreateRendererList(rendererListParams);

            // 유효한 RendererList를 만들 수 없다면 이 패스를 기록하지 않습니다.
            if (!passData.rendererList.IsValid())
                return;

            // 이 패스가 rendererList를 사용한다는 의존성을 RenderGraph에 알립니다.
            builder.UseRendererList(passData.rendererList);

            // 마스크 텍스처를 0번 색상 출력 대상으로 지정합니다.
            builder.SetRenderAttachment(maskTexture, 0, AccessFlags.Write);

            // 기존 카메라 Depth Texture를 읽기 전용 깊이 대상으로 연결합니다.
            // Mask Shader의 ZTest LEqual과 함께 가려진 픽셀을 제거합니다.
            builder.SetRenderAttachmentDepth(
                resourceData.activeDepthTexture,
                AccessFlags.Read);

            // 실제 GPU 명령을 실행할 함수를 등록합니다.
            builder.SetRenderFunc(static (
                MaskPassData data,
                RasterGraphContext context) =>
            {
                // 필터링된 선택 시설 Renderer를 마스크 텍스처에 그립니다.
                context.cmd.DrawRendererList(data.rendererList);
            });
        }

        /// <summary>
        /// 카메라 색상과 선택 모델 마스크를 읽어 외곽선을 최종 화면에 합성합니다.
        /// </summary>
        private void RecordCompositePass(
            RenderGraph renderGraph,
            UniversalResourceData resourceData,
            TextureHandle sourceTexture,
            TextureHandle maskTexture)
        {
            // RenderGraph에 전체 화면 합성용 래스터 패스를 추가합니다.
            using IRasterRenderGraphBuilder builder =
                renderGraph.AddRasterRenderPass<CompositePassData>(
                    "Facility Outline Composite",
                    out CompositePassData passData);

            // 합성 셰이더가 읽을 카메라 색상 복사본을 전달합니다.
            passData.sourceTexture = sourceTexture;

            // 합성 셰이더가 읽을 선택 모델 마스크를 전달합니다.
            passData.maskTexture = maskTexture;

            // 1번 Composite Pass가 들어 있는 머티리얼을 전달합니다.
            passData.material = material;

            // 최종 외곽선 색상을 전달합니다.
            passData.color = color;

            // 픽셀 단위 외곽선 두께를 전달합니다.
            passData.width = width;

            // 외곽선 부드러움 값을 전달합니다.
            passData.softness = softness;

            // 카메라 색상 복사본을 읽는다는 의존성을 선언합니다.
            builder.UseTexture(sourceTexture, AccessFlags.Read);

            // 선택 모델 마스크를 읽는다는 의존성을 선언합니다.
            builder.UseTexture(maskTexture, AccessFlags.Read);

            // 합성 결과를 현재 카메라 Color Texture에 씁니다.
            builder.SetRenderAttachment(
                resourceData.activeColorTexture,
                0,
                AccessFlags.Write);

            // 실제 전체 화면 합성 명령을 실행할 함수를 등록합니다.
            builder.SetRenderFunc(static (
                CompositePassData data,
                RasterGraphContext context) =>
            {
                // 이번 프레임의 마스크 텍스처를 셰이더 프로퍼티에 연결합니다.
                data.material.SetTexture(MaskTextureId, data.maskTexture);

                // Inspector에서 정한 외곽선 색상을 셰이더에 전달합니다.
                data.material.SetColor(OutlineColorId, data.color);

                // Inspector에서 정한 외곽선 두께를 셰이더에 전달합니다.
                data.material.SetFloat(OutlineWidthId, data.width);

                // Inspector에서 정한 부드러움 값을 셰이더에 전달합니다.
                data.material.SetFloat(OutlineSoftnessId, data.softness);

                // 화면 전체를 한 번 그립니다.
                // sourceTexture는 원본 화면이고 머티리얼의 1번 Pass가 외곽선을 합성합니다.
                Blitter.BlitTexture(
                    context.cmd,
                    data.sourceTexture,
                    Vector2.one,
                    data.material,
                    1);
            });
        }

        /// <summary>
        /// Mask Pass 실행 시 필요한 데이터 묶음입니다.
        /// RenderGraph가 실행 시점까지 이 데이터를 보관합니다.
        /// </summary>
        private sealed class MaskPassData
        {
            // 선택된 시설 Renderer 목록입니다.
            internal RendererListHandle rendererList;
        }

        /// <summary>
        /// Composite Pass 실행 시 필요한 데이터 묶음입니다.
        /// RenderGraph의 실행 함수에는 외부 지역 변수를 직접 캡처하지 않고 이 데이터로 넘깁니다.
        /// </summary>
        private sealed class CompositePassData
        {
            // 외곽선을 합성하기 전 원본 카메라 색상입니다.
            internal TextureHandle sourceTexture;

            // 선택 모델이 흰색으로 들어 있는 마스크입니다.
            internal TextureHandle maskTexture;

            // FacilityOutline.shader를 사용하는 머티리얼입니다.
            internal Material material;

            // 최종 외곽선 색상입니다.
            internal Color color;

            // 화면 픽셀 기준 외곽선 두께입니다.
            internal float width;

            // 외곽선 바깥쪽의 부드러움입니다.
            internal float softness;
        }
    }
}
