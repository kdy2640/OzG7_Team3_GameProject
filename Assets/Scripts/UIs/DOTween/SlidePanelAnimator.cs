using System.Collections;
using DG.Tweening;
using UnityEngine;

public sealed class SlidePanelAnimator : PanelAnimator
{
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private Vector2 startOffset = new(301f, 0f);

    private Vector2 endPosition;
    private Vector3 endEuler;
    private Vector3 endScale;

    protected override void Awake()
    {
        base.Awake();

        endPosition = Rect.anchoredPosition;
        endEuler = Rect.localEulerAngles;
        endScale = Rect.localScale;

        ResetToStart();
    }

    public override IEnumerator Show()
    {
        DOTween.Kill(this);
        ResetToStart();

        Sequence sequence = DOTween.Sequence()
            .SetTarget(this)
            .SetAutoKill(false)
            .Join(Rect.DOAnchorPos(endPosition, duration).SetEase(Ease.OutExpo));

        sequence.OnComplete(() => CanvasGroup.blocksRaycasts = true);
        sequence.OnKill(RestoreEndState);

        return sequence.WaitForCompletion(true);
    }

    public override IEnumerator Hide()
    {
        DOTween.Kill(this);

        Vector2 startPosition = Rect.anchoredPosition;
        bool startBlocksRaycasts = CanvasGroup.blocksRaycasts;

        CanvasGroup.blocksRaycasts = false;

        Sequence sequence = DOTween.Sequence()
            .SetTarget(this)
            .SetAutoKill(false)
            .Join(Rect.DOAnchorPos(
                endPosition + startOffset,
                duration * 0.3f).SetEase(Ease.InCubic));

        sequence.OnKill(Restore);

        void Restore()
        {
            Rect.anchoredPosition = startPosition;
            CanvasGroup.blocksRaycasts = startBlocksRaycasts;
        }

        return sequence.WaitForCompletion(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetToStart();
    }

    private void ResetToStart()
    {
        Rect.anchoredPosition = endPosition + startOffset;
        Rect.localEulerAngles = endEuler;
        Rect.localScale = endScale;
        CanvasGroup.alpha = 1f;
        CanvasGroup.blocksRaycasts = false;
    }

    private void RestoreEndState()
    {
        Rect.anchoredPosition = endPosition;
        Rect.localEulerAngles = endEuler;
        Rect.localScale = endScale;
        CanvasGroup.alpha = 1f;
        CanvasGroup.blocksRaycasts = true;
    }
}
