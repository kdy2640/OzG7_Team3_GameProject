using DG.Tweening;
using UnityEngine;

public sealed class FallingPanelAnimator : TweenPanelAnimator
{
    [Header("Falling Position")]
    [SerializeField] private Vector2 startOffset = new(0f, 180f);
    [SerializeField] private Vector2 curveOffset = new(-60f, -30f);

    [Header("Falling Transform")]
    [SerializeField] private float startRotation = -8f;
    [SerializeField] private float startScale = 0.8f;

    [Header("Falling Alpha")]
    [SerializeField, Range(0f, 1f)]
    private float startAlpha = 0.2f;

    public override Tween Show()
    {
        PrepareAnimation();

        Vector2 startPos = targetPos + startOffset;

        rect.anchoredPosition = startPos;
        rect.localEulerAngles = targetEuler + new Vector3(0f, 0f, startRotation);

        rect.localScale = targetScale * startScale;
        canvasGroup.alpha = startAlpha;

        Sequence sequence = CreateSequence();

        Vector2 p0 = startPos;
        Vector2 p1 = Vector2.Lerp(startPos, targetPos, 0.35f) + curveOffset;

        Vector2 p2 = Vector2.Lerp(startPos, targetPos, 0.75f) + curveOffset * 0.25f;

        Vector2 p3 = targetPos;

        sequence.Join(DOTween.To(() => 0f,
                t => rect.anchoredPosition =
                    Bezier(p0, p1, p2, p3, t),1f,duration
            ).SetEase(Ease.OutCubic));

        sequence.Join(rect.DOLocalRotate(targetEuler,duration
            ).SetEase(Ease.OutCubic));

        sequence.Join(rect.DOScale(targetScale,duration
            ).SetEase(Ease.OutCubic));

        sequence.Join(FadeIn(1f, duration * 0.8f));

        return CompleteShow(sequence);
    }

    public override Tween Hide()
    {
        PrepareAnimation();

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(targetPos + new Vector2(0f, 40f),duration * 0.35f
                ).SetEase(Ease.InCubic))
            .Join(
                FadeOut(duration * 0.35f));
    }
}