using DG.Tweening;
using UnityEngine;

public sealed class LeftPanelAnimator : PanelAnimator
{
    [SerializeField] private float startOffsetX = -420f;

    public override Tween Show()
    {
        SaveTargetTransform();

        rect.anchoredPosition = targetPos + new Vector2(startOffsetX, 0);
        rect.localScale = targetScale * 0.95f;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        if (delay > 0)
            seq.AppendInterval(delay);

        seq.Join(rect.DOAnchorPos(targetPos, duration).SetEase(Ease.OutBack));
        seq.Join(rect.DOScale(targetScale, duration).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, duration * 0.75f).SetEase(Ease.OutExpo));

        seq.OnComplete(() => canvasGroup.blocksRaycasts = true);

        return seq;
    }

    public override Tween Hide()
    {
        canvasGroup.blocksRaycasts = false;

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(targetPos + new Vector2(startOffsetX, 0), duration * 0.3f).SetEase(Ease.InCubic))
            .Join(canvasGroup.DOFade(0f, duration * 0.3f));
    }
}