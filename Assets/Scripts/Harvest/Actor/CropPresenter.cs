using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CropPresenter : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField, Min(0f)] private float shakeDuration = 0.2f;
    [SerializeField, Min(0f)] private float shakeAmplitude = 0.15f;

    private Vector3 originLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        originLocalPosition = visualRoot.localPosition;
    }

    private void OnDisable()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        if (visualRoot != null)
        {
            visualRoot.localPosition = originLocalPosition;
        }
    }

    public void PlayHit()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    public void Disappear()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * shakeAmplitude;
            visualRoot.localPosition =
                originLocalPosition + new Vector3(offset.x, 0f, offset.y);
            yield return null;
        }

        visualRoot.localPosition = originLocalPosition;
        shakeCoroutine = null;
    }
}
