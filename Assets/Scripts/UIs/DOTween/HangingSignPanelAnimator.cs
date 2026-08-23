using DG.Tweening;
using UnityEngine;

//간판이 위에서 내려와 흔들리는 형태
public sealed class HangingSignPanelAnimator : PanelAnimator
{
    [SerializeField] private Vector2 startOffset = new(0, 220);

    public override Tween Show()
    {
        SaveTargetTransform();

        rect.anchoredPosition = targetPos + startOffset;
        rect.localScale = targetScale * 0.85f;
        rect.localEulerAngles = targetEuler + new Vector3(0, 0, -18);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        if (delay > 0)
            seq.AppendInterval(delay);

        seq.Join(rect.DOAnchorPos(targetPos, duration).SetEase(Ease.OutExpo));
        seq.Join(canvasGroup.DOFade(1f, duration * 0.8f).SetEase(Ease.OutExpo));
        seq.Join(rect.DOScale(targetScale, duration).SetEase(Ease.OutBack));

        seq.Append(rect.DOLocalRotate(targetEuler + new Vector3(0, 0, 8), 0.10f).SetEase(Ease.OutSine));
        seq.Append(rect.DOLocalRotate(targetEuler + new Vector3(0, 0, -5), 0.12f).SetEase(Ease.InOutSine));
        seq.Append(rect.DOLocalRotate(targetEuler + new Vector3(0, 0, 3), 0.12f).SetEase(Ease.InOutSine));
        seq.Append(rect.DOLocalRotate(targetEuler, 0.16f).SetEase(Ease.OutCubic));

        seq.OnComplete(() => canvasGroup.blocksRaycasts = true);

        return seq;
    }

    public override Tween Hide()
    {
        canvasGroup.blocksRaycasts = false;

        return DOTween.Sequence()
            .Join(canvasGroup.DOFade(0f, duration * 0.3f))
            .Join(rect.DOScale(targetScale * 0.9f, duration * 0.3f));
    }
}