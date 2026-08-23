using DG.Tweening;
using UnityEngine;

//살짝 위에서 등장
public sealed class FloatPanelAnimator : PanelAnimator
{
    [SerializeField] private Vector2 startOffset = new(0, 35);

    public override Tween Show()
    {
        SaveTargetTransform();

        rect.anchoredPosition = targetPos + startOffset;
        rect.localScale = targetScale * 0.8f;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        if (delay > 0) seq.AppendInterval(delay);

        seq.Join(rect.DOAnchorPos(targetPos, duration).SetEase(Ease.OutExpo));
        seq.Join(rect.DOScale(targetScale, duration).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, duration * 0.75f).SetEase(Ease.OutExpo));

        seq.OnComplete(() => canvasGroup.blocksRaycasts = true);

        return seq;
    }

    public override Tween Hide()
    {
        canvasGroup.blocksRaycasts = false;

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(targetPos + startOffset * 0.5f, duration * 0.3f).SetEase(Ease.InCubic))
            .Join(canvasGroup.DOFade(0f, duration * 0.3f))
            .Join(rect.DOScale(targetScale * 0.9f, duration * 0.3f));
    }
}