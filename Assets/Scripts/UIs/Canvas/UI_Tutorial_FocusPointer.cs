using UnityEngine;
using UnityEngine.UI;

public class UI_Tutorial_FocusPointer : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("강조할 타겟 UI 오브젝트")]
    public RectTransform targetUI;

    [Header("Tutorial Elements")]
    [Tooltip("Hole_Cutter")]
    public RectTransform holeCutter;

    [Tooltip("Hole_Outline")]
    public RectTransform holeOutline;

    [Header("Outline Settings")]
    [Tooltip("외곽선의 크기 배수")]
    public float outlineScaleMultiplier = 1.1f;

    // 타겟 UI의 모든 정보를 복사하여 튜토리얼 마스크에 적용합니다.
    [ContextMenu("Update Focus (테스트 실행)")] // 인스펙터 우클릭으로 씬 뷰에서 바로 테스트 가능!
    public void UpdateFocus()
    {
        if (targetUI == null || holeCutter == null || holeOutline == null)
        {
            Debug.LogWarning("타겟 UI나 Cutter, Outline이 할당되지 않았습니다.");
            return;
        }

        // 1. 이미지(Sprite) 및 그리기 설정 복사
        Image targetImage = targetUI.GetComponent<Image>();
        Image cutterImage = holeCutter.GetComponent<Image>();
        Image outlineImage = holeOutline.GetComponent<Image>();

        if (targetImage != null)
        {
            if (cutterImage != null) CopyImageSettings(targetImage, cutterImage);
            if (outlineImage != null) CopyImageSettings(targetImage, outlineImage);
        }

        // 2. 피벗(Pivot)과 앵커(Anchor) 동기화
        // 타겟의 피벗과 앵커가 다르면 위치가 어긋날 수 있으므로 1차로 맞춰줍니다.
        SyncRectTransformBase(targetUI, holeCutter);
        SyncRectTransformBase(targetUI, holeOutline);

        // 3. Transform 복사 (위치, 회전)
        // Canvas의 계층구조가 다를 수 있으므로 화면 기준 월드 좌표를 사용합니다.
        holeCutter.position = targetUI.position;
        holeCutter.rotation = targetUI.rotation;

        holeOutline.position = targetUI.position;
        holeOutline.rotation = targetUI.rotation;

        // 4. 실제 크기(Size) 복사
        // 앵커의 영향을 받은 최종 렌더링 크기를 가져옵니다.
        Vector2 actualSize = new Vector2(targetUI.rect.width, targetUI.rect.height);
        holeCutter.sizeDelta = actualSize;
        holeOutline.sizeDelta = actualSize;

        // 5. 스케일(Scale) 복사 및 외곽선 스케일 업
        holeCutter.localScale = targetUI.localScale;
        holeOutline.localScale = targetUI.localScale * outlineScaleMultiplier;
    }


    private void CopyImageSettings(Image source, Image destination)
    {
        destination.sprite = source.sprite;
        destination.type = source.type; // Sliced 등 이미지 타입도 똑같이 맞춰 형태 찌그러짐 방지
        destination.pixelsPerUnitMultiplier = source.pixelsPerUnitMultiplier;
    }

    private void SyncRectTransformBase(RectTransform source, RectTransform destination)
    {
        destination.pivot = source.pivot;
        destination.anchorMin = new Vector2(0.5f, 0.5f);
        destination.anchorMax = new Vector2(0.5f, 0.5f);
    }
}