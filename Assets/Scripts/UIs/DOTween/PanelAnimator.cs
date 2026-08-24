using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class PanelAnimator : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected float duration = 0.6f;
    [SerializeField] protected float delay = 0f;

    [Header("Auto Play")]
    [SerializeField] private bool playOnParentShow = true;

    protected RectTransform rect;
    protected CanvasGroup canvasGroup;

    protected Vector2 targetPos;
    protected Vector3 targetScale;
    protected Vector3 targetEuler;

    private bool isInitialized;

    public bool PlayOnParentShow => playOnParentShow;

    protected virtual void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (isInitialized)
            return;

        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        targetPos = rect.anchoredPosition;
        targetScale = rect.localScale;
        targetEuler = rect.localEulerAngles;

        isInitialized = true;
    }

    protected void PrepareAnimation()
    {
        Initialize();

        rect.DOKill();
        canvasGroup.DOKill();

        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void OnDisable()
    {
        if (!isInitialized)
            return;

        rect.DOKill();
        canvasGroup.DOKill();

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public abstract Tween Show();

    public abstract Tween Hide();

    public void SetDelay(float value)
    {
        delay = value;
    }
}