using DG.Tweening;
using UnityEngine;

public abstract class TweenPanelAnimator : PanelAnimator
{
    protected Sequence CreateSequence()
    {
        Sequence sequence = DOTween.Sequence();

        if (delay > 0f) sequence.AppendInterval(delay);

        return sequence;
    }

    protected Tween CompleteShow(Sequence sequence)
    {
        sequence.OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
        });

        return sequence;
    }

    protected Tween FadeIn(float endAlpha, float fadeDuration)
    {
        return canvasGroup.DOFade(endAlpha, fadeDuration)
            .SetEase(Ease.OutExpo);
    }

    protected Tween FadeOut(float fadeDuration)
    {
        return canvasGroup.DOFade(0f, fadeDuration)
            .SetEase(Ease.InQuad);
    }

    protected static Vector2 Bezier(
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