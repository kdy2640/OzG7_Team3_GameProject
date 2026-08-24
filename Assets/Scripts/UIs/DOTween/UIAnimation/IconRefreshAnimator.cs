using DG.Tweening;
using UnityEngine;

public sealed class IconRefreshAnimator : UIRefreshAnimator
{
    public override Tween Refresh()
    {
        rect.DOKill();

        rect.localScale = targetScale * 0.9f;

        return rect.DOScale(targetScale, duration)
            .SetEase(Ease.OutBack);
    }
}