using DG.Tweening;
using UnityEngine;

public sealed class TextRefreshAnimator : UIRefreshAnimator
{
    [SerializeField] private float moveY = 8f;

    public override Tween Refresh()
    {
        rect.DOKill();
        canvasGroup.DOKill();

        rect.localScale = targetScale * 0.97f;
        rect.anchoredPosition += Vector2.up * moveY;
        canvasGroup.alpha = 0.4f;

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(rect.anchoredPosition - Vector2.up * moveY, duration)
                .SetEase(Ease.OutCubic))
            .Join(rect.DOScale(targetScale, duration)
                .SetEase(Ease.OutBack))
            .Join(canvasGroup.DOFade(1f, duration * 0.8f)
                .SetEase(Ease.OutExpo));
    }
}