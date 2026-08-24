using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class UIRefreshAnimator : MonoBehaviour
{
    [SerializeField] protected float duration = 0.25f;

    protected RectTransform rect;
    protected CanvasGroup canvasGroup;
    protected Vector3 targetScale;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        targetScale = rect.localScale;
    }

    public abstract Tween Refresh();
}