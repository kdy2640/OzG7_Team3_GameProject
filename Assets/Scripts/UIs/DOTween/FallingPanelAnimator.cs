using System.Collections;
using DG.Tweening;
using UnityEngine;

public sealed class FallingPanelAnimator : PanelAnimator
{
    [SerializeField] private float duration = 0.6f;

    [Header("Position")]
    [SerializeField] private Vector2 startOffset = new(0f, 180f);
    [SerializeField] private Vector2 curveOffset = new(-60f, -30f);

    [Header("Transform")]
    [SerializeField] private float startRotation = -8f;
    [SerializeField] private float startScale = 0.8f;

    [Header("Alpha")]
    [SerializeField, Range(0f, 1f)] private float startAlpha = 0.2f;

    public override IEnumerator Show()
    {
        DOTween.Kill(this);

        Vector2 endPosition = Rect.anchoredPosition;
        Vector3 endEuler = Rect.localEulerAngles;
        Vector3 endScale = Rect.localScale;

        Vector2 startPosition = endPosition + startOffset;
        Vector2 curvePoint1 = Vector2.Lerp(startPosition, endPosition, 0.35f) + curveOffset;
        Vector2 curvePoint2 = Vector2.Lerp(startPosition, endPosition, 0.75f) + curveOffset * 0.25f;

        Rect.anchoredPosition = startPosition;
        Rect.localEulerAngles = endEuler + new Vector3(0f, 0f, startRotation);
        Rect.localScale = endScale * startScale;
        CanvasGroup.alpha = startAlpha;
        CanvasGroup.blocksRaycasts = false;

        Sequence sequence = DOTween.Sequence()
            .SetTarget(this)
            .SetAutoKill(false)
            .Join(DOTween.To(
                () => 0f,
                value => Rect.anchoredPosition = Bezier(
                    startPosition,
                    curvePoint1,
                    curvePoint2,
                    endPosition,
                    value),
                1f,
                duration).SetEase(Ease.OutCubic))
            .Join(Rect.DOLocalRotate(endEuler, duration).SetEase(Ease.OutCubic))
            .Join(Rect.DOScale(endScale, duration).SetEase(Ease.OutCubic))
            .Join(CanvasGroup.DOFade(1f, duration * 0.8f).SetEase(Ease.OutExpo));

        sequence.OnComplete(() => CanvasGroup.blocksRaycasts = true);
        sequence.OnKill(Restore);

        void Restore()
        {
            Rect.anchoredPosition = endPosition;
            Rect.localEulerAngles = endEuler;
            Rect.localScale = endScale;
            CanvasGroup.alpha = 1f;
            CanvasGroup.blocksRaycasts = true;
        }

        return sequence.WaitForCompletion(true);
    }

    public override IEnumerator Hide()
    {
        DOTween.Kill(this);

        Vector2 startPosition = Rect.anchoredPosition;
        Vector3 startEuler = Rect.localEulerAngles;
        Vector3 startScaleValue = Rect.localScale;
        float startAlphaValue = CanvasGroup.alpha;
        bool startBlocksRaycasts = CanvasGroup.blocksRaycasts;

        CanvasGroup.blocksRaycasts = false;

        Sequence sequence = DOTween.Sequence()
            .SetTarget(this)
            .SetAutoKill(false)
            .Join(Rect.DOAnchorPos(
                startPosition + new Vector2(0f, 40f),
                duration * 0.35f).SetEase(Ease.InCubic))
            .Join(CanvasGroup.DOFade(0f, duration * 0.35f).SetEase(Ease.InQuad));

        sequence.OnKill(Restore);

        void Restore()
        {
            Rect.anchoredPosition = startPosition;
            Rect.localEulerAngles = startEuler;
            Rect.localScale = startScaleValue;
            CanvasGroup.alpha = startAlphaValue;
            CanvasGroup.blocksRaycasts = startBlocksRaycasts;
        }

        return sequence.WaitForCompletion(true);
    }

    private static Vector2 Bezier(
        Vector2 p0,
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        float t)
    {
        float u = 1f - t;

        return
            u * u * u * p0 +
            3f * u * u * t * p1 +
            3f * u * t * t * p2 +
            t * t * t * p3;
    }
}
