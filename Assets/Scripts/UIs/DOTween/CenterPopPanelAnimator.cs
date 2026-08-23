using DG.Tweening;
using UnityEngine;

public sealed class CenterPopPanelAnimator : PanelAnimator
{
    [Header("Dim")]
    [SerializeField] private CanvasGroup dimCanvasGroup;

    public override Tween Show()
    {
        SaveTargetTransform();

        rect.localScale = targetScale * 0.8f;
        rect.localEulerAngles = targetEuler;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        if (dimCanvasGroup != null)
        {
            dimCanvasGroup.alpha = 0f;
            dimCanvasGroup.gameObject.SetActive(true);
        }

        Sequence seq = DOTween.Sequence();

        if (delay > 0)
            seq.AppendInterval(delay);

        if (dimCanvasGroup != null)
            seq.Join(dimCanvasGroup.DOFade(0.6f, duration * 0.6f).SetEase(Ease.OutQuad));

        seq.Join(canvasGroup.DOFade(1f, duration * 0.5f).SetEase(Ease.OutExpo));

        seq.Append(rect.DOScale(targetScale * 1.05f, duration * 0.45f).SetEase(Ease.OutBack));
        seq.Append(rect.DOScale(targetScale, 0.08f).SetEase(Ease.OutCubic));

        seq.OnComplete(() => canvasGroup.blocksRaycasts = true);

        return seq;
    }

    public override Tween Hide()
    {
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOScale(targetScale * 0.9f, duration * 0.25f).SetEase(Ease.InBack));
        seq.Join(canvasGroup.DOFade(0f, duration * 0.25f));

        if (dimCanvasGroup != null)
        {
            seq.Join(dimCanvasGroup.DOFade(0f, duration * 0.25f))
               .OnComplete(() => dimCanvasGroup.gameObject.SetActive(false));
        }

        return seq;
    }
}