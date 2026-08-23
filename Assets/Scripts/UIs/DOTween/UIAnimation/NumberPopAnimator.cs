using DG.Tweening;
using UnityEngine;

public sealed class NumberPopAnimator : UIRefreshAnimator
{
    public override Tween Refresh()
    {
        rect.DOKill();

        rect.localScale = targetScale;

        return DOTween.Sequence()
            .Append(rect.DOScale(targetScale * 1.08f, duration * 0.45f)
                .SetEase(Ease.OutBack))
            .Append(rect.DOScale(targetScale, duration * 0.55f)
                .SetEase(Ease.OutCubic));
    }
}