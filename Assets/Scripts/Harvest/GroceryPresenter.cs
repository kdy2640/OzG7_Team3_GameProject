using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GroceryPresenter : Poolable
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer groceryRenderer;
    [SerializeField, Min(0f)] private float baseScale = 1f;

    [Header("Random Adjustment")]
    [SerializeField, Min(0f)] private float randomRotationRange = 25f;
    [SerializeField, Range(0f, 1f)] private float randomScaleRange = 0.2f;

    [Header("Spawn Area")]
    [SerializeField, Min(0f)] private float spawnHalfWidth = 0.35f;
    [SerializeField, Min(0f)] private float spawnMinUpOffset = 0.25f;
    [SerializeField, Min(0f)] private float spawnMaxUpOffset = 0.65f;

    [Header("Popup")]
    [SerializeField, Min(0f)] private float popupDuration = 0.18f;
    [SerializeField, Min(0f)] private float popupStartScale = 0.2f;
    [SerializeField, Min(0f)] private float popupStayDuration = 0.1f;

    [Header("Move")]
    [SerializeField, Min(0f)] private float moveDuration = 0.45f;
    [SerializeField, Min(0f)] private float moveEndScale = 0.35f;
    [SerializeField, Min(0f)] private float moveArcHeight = 0.35f;

    private Sequence currentSequence;
    private Transform cameraTransform;
    private Vector3 targetVisualScale = Vector3.one;

    public override void Initialize(PoolArgs args)
    {
        GroceryArgs groceryArgs = (GroceryArgs)args;
        SetData(groceryArgs.GroceryType, groceryArgs.WorldPosition);
    }

    public IEnumerator PopUpRoutine()
    {
        StopCurrentTween();
        groceryRenderer.gameObject.SetActive(true);
        groceryRenderer.transform.localScale =
            targetVisualScale * popupStartScale;

        currentSequence = DOTween.Sequence();
        currentSequence.Append(
            groceryRenderer.transform
                .DOScale(targetVisualScale, popupDuration)
                .SetEase(Ease.OutBack));
        currentSequence.AppendInterval(popupStayDuration);

        yield return currentSequence.WaitForCompletion();
        currentSequence = null;
    }

    public IEnumerator MoveToTarget(Transform target)
    {
        StopCurrentTween();
        groceryRenderer.gameObject.SetActive(true);

        Vector3 startPosition = transform.position;
        Vector3 startScale = groceryRenderer.transform.localScale;
        Vector3 endScale = targetVisualScale * moveEndScale;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float time = Mathf.Clamp01(elapsed / moveDuration);
            float moveTime = time * time;
            Vector3 targetPosition = target.position;
            Vector3 arcOffset = cameraTransform.up
                * (Mathf.Sin(time * Mathf.PI) * moveArcHeight);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                moveTime) + arcOffset;
            groceryRenderer.transform.localScale = Vector3.Lerp(
                startScale,
                endScale,
                moveTime);

            yield return null;
        }

        transform.position = target.position;
        groceryRenderer.transform.localScale = endScale;
        groceryRenderer.gameObject.SetActive(false);
        RequestReturn();
    }

    public override void ResetState()
    {
        StopCurrentTween();

        groceryRenderer.gameObject.SetActive(false);
        groceryRenderer.sprite = null;
        groceryRenderer.transform.localPosition = Vector3.zero;
        groceryRenderer.transform.localRotation = Quaternion.identity;
        groceryRenderer.transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        cameraTransform = null;
        targetVisualScale = Vector3.one;
    }

    private void LateUpdate()
    {
        transform.rotation = cameraTransform.rotation;
    }

    private void SetData(GroceryType groceryType, Vector3 worldPosition)
    {
        StopCurrentTween();

        cameraTransform = Camera.main.transform;
        transform.SetPositionAndRotation(
            worldPosition,
            cameraTransform.rotation);
        transform.localScale = Vector3.one;

        ResetVisual();
        groceryRenderer.sprite = GroceryDataDB.GetData(groceryType).Icon;
        ApplyRandomAdjustment();
        groceryRenderer.gameObject.SetActive(false);
    }

    private void ResetVisual()
    {
        groceryRenderer.transform.localPosition = Vector3.zero;
        groceryRenderer.transform.localRotation = Quaternion.identity;
        groceryRenderer.transform.localScale = Vector3.one;

        Color color = groceryRenderer.color;
        color.a = 1f;
        groceryRenderer.color = color;
    }

    private void ApplyRandomAdjustment()
    {
        float randomRotation = Random.Range(
            -randomRotationRange,
            randomRotationRange);
        float randomScale = Random.Range(
            1f - randomScaleRange,
            1f + randomScaleRange);
        float randomX = Random.Range(-spawnHalfWidth, spawnHalfWidth);
        float randomY = Random.Range(
            spawnMinUpOffset,
            spawnMaxUpOffset);

        transform.position += cameraTransform.right * randomX
            + cameraTransform.up * randomY;
        groceryRenderer.transform.localRotation =
            Quaternion.Euler(0f, 0f, randomRotation);
        targetVisualScale = Vector3.one * baseScale * randomScale;
    }

    private void StopCurrentTween()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }
    }

    private void OnDestroy()
    {
        StopCurrentTween();
    }
}
