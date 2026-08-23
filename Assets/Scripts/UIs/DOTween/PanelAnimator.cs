using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class PanelAnimator : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected float duration = 0.6f;
    [SerializeField] protected float delay = 0f;

    protected RectTransform rect;
    protected CanvasGroup canvasGroup;

    protected Vector2 targetPos;
    protected Vector3 targetScale;
    protected Vector3 targetEuler;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        SaveTargetTransform();
    }

    protected void SaveTargetTransform()
    {
        targetPos = rect.anchoredPosition;
        targetScale = rect.localScale;
        targetEuler = rect.localEulerAngles;
    }

    protected virtual void OnDisable()
    {
        rect.DOKill();
        canvasGroup.DOKill();

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public abstract Tween Show();
    public abstract Tween Hide();

    public void SetDelay(float value) => delay = value;
}