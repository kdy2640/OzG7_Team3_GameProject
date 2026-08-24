using DG.Tweening;
using UnityEngine;

public sealed class TweenPanelAnimator : PanelAnimator
{
    private enum AnimationType
    {
        Falling,
        Float,
        Edge,
        HangingSign,
        CenterPop
    }

    private enum Direction
    {
        Left,
        Right
    }

    [Header("Animation Type")]
    [SerializeField] private AnimationType animationType = AnimationType.Float;

    [Header("Edge Direction - Edge 전용")]
    [SerializeField] private Direction direction = Direction.Left;

    [Header("Position - Falling / Float / HangingSign")]
    [SerializeField] private Vector2 startOffset = new(0f, 180f);

    [Header("Position - Edge 전용")]
    [SerializeField] private float edgeOffsetX = 420f;

    [Header("Position - Falling 전용")]
    [SerializeField] private Vector2 curveOffset = new(-60f, -30f);

    [Header("Transform - Falling 전용")]
    [SerializeField] private float startRotation = -8f;

    [Header("Transform - Falling / Float / HangingSign")]
    [SerializeField] private float startScale = 0.92f;

    [Header("Alpha - Falling 전용")]
    [SerializeField, Range(0f, 1f)]
    private float startAlpha = 0.2f;

    [Header("HangingSign 전용")]
    [SerializeField] private float hangingStartRotation = -18f;

    [Header("CenterPop 전용")]
    [SerializeField] private float centerStartScale = 0.8f;

    [Header("CenterPop 전용")]
    [SerializeField] private CanvasGroup dimCanvasGroup;

    [Header("CenterPop 전용")]
    [SerializeField, Range(0f, 1f)]
    private float dimAlpha = 0.6f;

    public override Tween Show()
    {
        return animationType switch
        {
            AnimationType.Falling => ShowFalling(),
            AnimationType.Float => ShowFloat(),
            AnimationType.Edge => ShowEdge(),
            AnimationType.HangingSign => ShowHangingSign(),
            AnimationType.CenterPop => ShowCenterPop(),
            _ => null
        };
    }

    public override Tween Hide()
    {
        return animationType switch
        {
            AnimationType.Falling => HideFalling(),
            AnimationType.Float => HideFloat(),
            AnimationType.Edge => HideEdge(),
            AnimationType.HangingSign => HideHangingSign(),
            AnimationType.CenterPop => HideCenterPop(),
            _ => null
        };
    }

    // =========================================================
    // Falling
    // =========================================================

    private Tween ShowFalling()
    {
        PrepareAnimation();

        Vector2 startPos = targetPos + startOffset;

        rect.anchoredPosition = startPos;
        rect.localEulerAngles = targetEuler + new Vector3(0f, 0f, startRotation);

        rect.localScale = targetScale * startScale;
        canvasGroup.alpha = startAlpha;

        Sequence seq = CreateSequence();

        Vector2 p0 = startPos;
        Vector2 p1 = Vector2.Lerp(startPos, targetPos, 0.35f) + curveOffset;

        Vector2 p2 =
            Vector2.Lerp(startPos, targetPos, 0.75f) + curveOffset * 0.25f;

        Vector2 p3 = targetPos;

        seq.Join(DOTween.To(() => 0f,
                t => rect.anchoredPosition =
                    Bezier(p0, p1, p2, p3, t),
                1f,duration).SetEase(Ease.OutCubic)
        );

        seq.Join(rect.DOLocalRotate(
                targetEuler,duration)
            .SetEase(Ease.OutCubic)
        );

        seq.Join(rect.DOScale(
                targetScale,duration)
            .SetEase(Ease.OutCubic)
        );

        seq.Join(canvasGroup.DOFade(
                1f,duration * 0.8f)
            .SetEase(Ease.OutExpo)
        );

        return CompleteShow(seq);
    }

    private Tween HideFalling()
    {
        PrepareAnimation();

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(
                    targetPos + new Vector2(0f, 40f),
                    duration * 0.35f).SetEase(Ease.InCubic)
            )
            .Join(canvasGroup.DOFade(
                    0f,duration * 0.35f).SetEase(Ease.InQuad)
            );
    }

    // =========================================================
    // Float
    // =========================================================

    private Tween ShowFloat()
    {
        PrepareAnimation();

        rect.anchoredPosition = targetPos + startOffset;
        rect.localEulerAngles = targetEuler;
        rect.localScale = targetScale * startScale;

        canvasGroup.alpha = 0f;

        Sequence seq = CreateSequence();

        seq.Join(rect.DOAnchorPos(
                targetPos,duration).SetEase(Ease.OutExpo)
        );

        seq.Join(rect.DOScale(
                targetScale,duration).SetEase(Ease.OutCubic)
        );

        seq.Join(canvasGroup.DOFade(
                1f,duration * 0.75f).SetEase(Ease.OutExpo)
        );

        return CompleteShow(seq);
    }

    private Tween HideFloat()
    {
        PrepareAnimation();

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(
                    targetPos + startOffset * 0.5f,
                    duration * 0.3f).SetEase(Ease.InCubic)
            )
            .Join(rect.DOScale(
                    targetScale * 0.95f,
                    duration * 0.3f).SetEase(Ease.InCubic)
            )
            .Join(canvasGroup.DOFade(
                    0f,duration * 0.3f).SetEase(Ease.InQuad)
            );
    }

    // =========================================================
    // Edge
    // =========================================================

    private Tween ShowEdge()
    {
        PrepareAnimation();

        float offsetX =
            direction == Direction.Left
                ? -Mathf.Abs(edgeOffsetX): Mathf.Abs(edgeOffsetX);

        Vector2 startPos =targetPos + new Vector2(offsetX, 0f);

        rect.anchoredPosition = startPos;
        rect.localEulerAngles = targetEuler;
        rect.localScale = targetScale * 0.95f;

        canvasGroup.alpha = 0f;

        Sequence seq = CreateSequence();

        seq.Join(rect.DOAnchorPos(
                targetPos,duration).SetEase(Ease.OutExpo)
        );

        seq.Join(rect.DOScale(targetScale,
                duration).SetEase(Ease.OutCubic)
        );

        seq.Join(canvasGroup.DOFade(
                1f,duration * 0.75f).SetEase(Ease.OutExpo)
        );

        return CompleteShow(seq);
    }

    private Tween HideEdge()
    {
        PrepareAnimation();

        float offsetX =
            direction == Direction.Left
                ? -Mathf.Abs(edgeOffsetX): Mathf.Abs(edgeOffsetX);

        Vector2 endPos =targetPos + new Vector2(offsetX, 0f);

        return DOTween.Sequence()
            .Join(rect.DOAnchorPos(
                    endPos,duration * 0.3f).SetEase(Ease.InCubic)
            )
            .Join(canvasGroup.DOFade(
                    0f,duration * 0.3f).SetEase(Ease.InQuad)
            );
    }

    // =========================================================
    // Hanging Sign
    // =========================================================

    private Tween ShowHangingSign()
    {
        PrepareAnimation();

        rect.anchoredPosition = targetPos + startOffset;

        rect.localScale = targetScale * startScale;

        rect.localEulerAngles =
            targetEuler + new Vector3(0f,0f,hangingStartRotation);

        canvasGroup.alpha = 0f;

        Sequence seq = CreateSequence();

        seq.Join(rect.DOAnchorPos(
                targetPos,duration).SetEase(Ease.OutExpo)
        );

        seq.Join(rect.DOScale(
                targetScale,duration).SetEase(Ease.OutCubic)
        );

        seq.Join(canvasGroup.DOFade(
                1f,duration * 0.8f).SetEase(Ease.OutExpo)
        );

        // 감쇠 흔들림
        seq.Append(rect.DOLocalRotate(
                targetEuler + new Vector3(0f, 0f, 8f),0.10f)
            .SetEase(Ease.OutSine)
        );

        seq.Append(rect.DOLocalRotate(
                targetEuler + new Vector3(0f, 0f, -5f),0.12f)
            .SetEase(Ease.InOutSine)
        );

        seq.Append(rect.DOLocalRotate(
                targetEuler + new Vector3(0f, 0f, 3f),0.12f)
            .SetEase(Ease.InOutSine)
        );

        seq.Append(rect.DOLocalRotate(targetEuler,0.16f)
            .SetEase(Ease.OutCubic)
        );

        return CompleteShow(seq);
    }

    private Tween HideHangingSign()
    {
        PrepareAnimation();

        return DOTween.Sequence()
            .Join(canvasGroup.DOFade(0f,duration * 0.3f)
                .SetEase(Ease.InQuad)
            )
            .Join(rect.DOScale(
                    targetScale * 0.9f,duration * 0.3f)
            .SetEase(Ease.InCubic)
            );
    }

    // =========================================================
    // Center Pop
    // =========================================================

    private Tween ShowCenterPop()
    {
        PrepareAnimation();

        rect.anchoredPosition = targetPos;
        rect.localEulerAngles = targetEuler;
        rect.localScale = targetScale * centerStartScale;

        canvasGroup.alpha = 0f;

        if (dimCanvasGroup != null)
        {
            dimCanvasGroup.gameObject.SetActive(true);
            dimCanvasGroup.alpha = 0f;
        }

        Sequence seq = CreateSequence();

        seq.Join(rect.DOScale(targetScale,duration)
            .SetEase(Ease.OutCubic)
        );

        seq.Join(canvasGroup.DOFade(1f,duration * 0.5f)
            .SetEase(Ease.OutExpo)
        );

        if (dimCanvasGroup != null)
        {
            seq.Join(dimCanvasGroup.DOFade(
                    dimAlpha,duration * 0.6f)
                .SetEase(Ease.OutCubic));
        }

        return CompleteShow(seq);
    }

    private Tween HideCenterPop()
    {
        PrepareAnimation();

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOScale(targetScale * 0.9f,duration * 0.25f)
            .SetEase(Ease.InCubic)
        );

        seq.Join(canvasGroup.DOFade(0f,duration * 0.25f)
            .SetEase(Ease.InQuad)
        );

        if (dimCanvasGroup != null)
        {
            seq.Join(dimCanvasGroup.DOFade(0f,duration * 0.25f)
                .SetEase(Ease.InQuad));

            seq.OnComplete(() =>
            {
                dimCanvasGroup.gameObject.SetActive(false);
            });
        }

        return seq;
    }

    // =========================================================
    // Common
    // =========================================================

    private Sequence CreateSequence()
    {
        Sequence seq = DOTween.Sequence();

        if (delay > 0f) seq.AppendInterval(delay);

        return seq;
    }

    private Tween CompleteShow(Sequence seq)
    {
        seq.OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
        });

        return seq;
    }

    private static Vector2 Bezier(
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