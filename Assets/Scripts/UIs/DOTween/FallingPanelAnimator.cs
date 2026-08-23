using DG.Tweening;
using UnityEngine;

public sealed class FallingPanelAnimator : PanelAnimator
{
    [Header("Falling")]
    [SerializeField] private Vector2 startOffset = new(0, 180);
    [SerializeField] private Vector2 curveOffset = new(-60, -30);
    [SerializeField] private float startRotation = -8f;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float startAlpha = 0.2f;

    public override Tween Show()
    {
        rect.DOKill();
        canvasGroup.DOKill();

        SaveTargetTransform();

        Vector2 startPos = targetPos + startOffset;

        rect.anchoredPosition = startPos;
        rect.localEulerAngles = targetEuler + new Vector3(0, 0, startRotation);
        rect.localScale = targetScale * startScale;

        canvasGroup.alpha = startAlpha;
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        if (delay > 0) seq.AppendInterval(delay);

        Vector2 p0 = startPos;
        Vector2 p1 = Vector2.Lerp(startPos, targetPos, 0.35f) + curveOffset;
        Vector2 p2 = Vector2.Lerp(startPos, targetPos, 0.75f) + curveOffset * 0.25f;
        Vector2 p3 = targetPos;

        seq.Join(DOTween.To(
            () => 0f,
            t => rect.anchoredPosition = Bezier(p0, p1, p2, p3, t),
            1f,
            duration).SetEase(Ease.OutCubic));

        seq.Join(rect.DOLocalRotate(targetEuler, duration).SetEase(Ease.OutCubic));
        seq.Join(rect.DOScale(targetScale, duration).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, duration * 0.8f).SetEase(Ease.OutExpo));

        seq.OnComplete(() => canvasGroup.blocksRaycasts = true);

        return seq;
    }

    public override Tween Hide()
    {
        canvasGroup.blocksRaycasts = false;

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(targetPos + new Vector2(0, 40), duration * 0.35f).SetEase(Ease.InCubic))
            .Join(canvasGroup.DOFade(0f, duration * 0.35f).SetEase(Ease.InQuad))
            .Join(rect.DOScale(targetScale * 0.9f, duration * 0.35f).SetEase(Ease.InCubic));
    }

    private static Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1f - t;
        return u * u * u * p0 + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * p3;
    }
}