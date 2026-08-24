using DG.Tweening;
using UnityEngine;

public sealed class FloatPanelAnimator : TweenPanelAnimator
{
    [Header("Float Position")]
    [SerializeField] private Vector2 startOffset = new(0f, 35f);

    [Header("Float Scale")]
    [SerializeField] private float startScale = 0.92f;

    public override Tween Show()
    {
        PrepareAnimation();

        rect.anchoredPosition = targetPos + startOffset;

        rect.localEulerAngles = targetEuler;
        rect.localScale = targetScale * startScale;

        canvasGroup.alpha = 0f;

        Sequence sequence = CreateSequence();

        sequence.Join(rect.DOAnchorPos(targetPos,duration
            ).SetEase(Ease.OutExpo));

        sequence.Join(rect.DOScale(targetScale,duration
            ).SetEase(Ease.OutCubic));

        sequence.Join(FadeIn(1f,duration * 0.75f));

        return CompleteShow(sequence);
    }

    public override Tween Hide()
    {
        PrepareAnimation();

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(targetPos + startOffset * 0.5f,duration * 0.3f
                    ).SetEase(Ease.InCubic))
            .Join(rect.DOScale(targetScale * 0.95f,duration * 0.3f
                ).SetEase(Ease.InCubic))
            .Join(FadeOut(duration * 0.3f));
    }
}