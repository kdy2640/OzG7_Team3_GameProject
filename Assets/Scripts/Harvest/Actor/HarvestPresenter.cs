using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class HarvestPresenter : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField, Min(0f)] private float hitDuration = 0.18f;
    [SerializeField, Min(0f)] private float bendAngle = 15f;
    [SerializeField, Range(0f, 1f)] private float squashHeight = 0.82f;
    [SerializeField, Min(1f)] private float squashWidth = 1.15f;
    [SerializeField, Min(1f)] private float stretchHeight = 1.08f;
    [SerializeField, Range(0f, 1f)] private float stretchWidth = 0.95f;
    [SerializeField, Min(0f)] private float deathDuration = 0.24f;
    [SerializeField, Range(0f, 90f)] private float deathBendAngle = 75f;
    [SerializeField, Min(0f)] private float deathDropDistance = 0.15f;

    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;
    private Vector3 originLocalScale;
    private Coroutine hitCoroutine;
    private Coroutine deathCoroutine;

    public void Init(GameObject solid)
    { 
        if (solid != null)
        {
            visualRoot = solid.transform; 
            originLocalPosition = visualRoot.localPosition;
            originLocalRotation = visualRoot.localRotation;
            originLocalScale = visualRoot.localScale;
        }
    }

    private void OnDisable()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }

        if (visualRoot != null)
        {
            visualRoot.localPosition = originLocalPosition;
            visualRoot.localRotation = originLocalRotation;
            visualRoot.localScale = originLocalScale;
        }
    }

    public void PlayHit()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        hitCoroutine = StartCoroutine(HitRoutine());
    }

    public void PlayDeath()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        deathCoroutine = StartCoroutine(DeathRoutine());
    }

    private IEnumerator HitRoutine()
    {
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Harvest_CropHit);
        Vector3 bendDirection = Camera.main.transform.forward;
        bendDirection.y = 0f;

        Vector3 localBendDirection =
            visualRoot.parent.InverseTransformDirection(bendDirection).normalized;
        Vector3 bendAxis =
            Vector3.Cross(Vector3.up, localBendDirection).normalized;

        Quaternion startRotation = visualRoot.localRotation;
        Vector3 startScale = visualRoot.localScale;
        Quaternion impactRotation =
            Quaternion.AngleAxis(bendAngle, bendAxis) * originLocalRotation;
        Quaternion recoilRotation =
            Quaternion.AngleAxis(-bendAngle * 0.25f, bendAxis)
            * originLocalRotation;
        Vector3 squashedScale = Vector3.Scale(
            originLocalScale,
            new Vector3(squashWidth, squashHeight, squashWidth));
        Vector3 stretchedScale = Vector3.Scale(
            originLocalScale,
            new Vector3(stretchWidth, stretchHeight, stretchWidth));

        float impactDuration = hitDuration * 0.25f;
        float recoilDuration = hitDuration * 0.35f;
        float settleDuration = hitDuration * 0.4f;
        float elapsed = 0f;

        while (elapsed < impactDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / impactDuration);
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            visualRoot.localRotation =
                Quaternion.Slerp(startRotation, impactRotation, easedT);
            visualRoot.localScale =
                Vector3.Lerp(startScale, squashedScale, easedT);
            yield return null;
        }

        visualRoot.localRotation = impactRotation;
        visualRoot.localScale = squashedScale;
        elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / recoilDuration);
            float easedT = t * t * (3f - 2f * t);

            visualRoot.localRotation =
                Quaternion.Slerp(impactRotation, recoilRotation, easedT);
            visualRoot.localScale =
                Vector3.Lerp(squashedScale, stretchedScale, easedT);
            yield return null;
        }

        visualRoot.localRotation = recoilRotation;
        visualRoot.localScale = stretchedScale;
        elapsed = 0f;

        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);
            float easedT = t * t * (3f - 2f * t);

            visualRoot.localRotation =
                Quaternion.Slerp(recoilRotation, originLocalRotation, easedT);
            visualRoot.localScale =
                Vector3.Lerp(stretchedScale, originLocalScale, easedT);
            yield return null;
        }

        visualRoot.localPosition = originLocalPosition;
        visualRoot.localRotation = originLocalRotation;
        visualRoot.localScale = originLocalScale;
        hitCoroutine = null;
    }

    private IEnumerator DeathRoutine()
    { 
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Harvest_CropHarvested);
        Vector3 bendDirection = Camera.main.transform.forward;
        bendDirection.y = 0f;

        Vector3 localBendDirection =
            visualRoot.parent.InverseTransformDirection(bendDirection).normalized;
        Vector3 bendAxis =
            Vector3.Cross(Vector3.up, localBendDirection).normalized;

        Vector3 startPosition = visualRoot.localPosition;
        Quaternion startRotation = visualRoot.localRotation;
        Vector3 startScale = visualRoot.localScale;
        Quaternion squashedRotation =
            Quaternion.AngleAxis(bendAngle * 1.5f, bendAxis)
            * originLocalRotation;
        Vector3 squashedScale = Vector3.Scale(
            originLocalScale,
            new Vector3(1.25f, 0.65f, 1.25f));
        Quaternion fallenRotation =
            Quaternion.AngleAxis(deathBendAngle, bendAxis)
            * originLocalRotation;
        Vector3 fallenPosition =
            originLocalPosition + Vector3.down * deathDropDistance;
        Vector3 flattenedScale = Vector3.Scale(
            originLocalScale,
            new Vector3(0.9f, 0.2f, 0.9f));

        float squashDuration = deathDuration * 0.25f;
        float fallDuration = deathDuration * 0.75f;
        float elapsed = 0f;

        while (elapsed < squashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / squashDuration);
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            visualRoot.localRotation =
                Quaternion.Slerp(startRotation, squashedRotation, easedT);
            visualRoot.localScale =
                Vector3.Lerp(startScale, squashedScale, easedT);
            yield return null;
        }

        visualRoot.localRotation = squashedRotation;
        visualRoot.localScale = squashedScale;
        elapsed = 0f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fallDuration);
            float easedT = t * t * (3f - 2f * t);

            visualRoot.localPosition =
                Vector3.Lerp(startPosition, fallenPosition, easedT);
            visualRoot.localRotation =
                Quaternion.Slerp(squashedRotation, fallenRotation, easedT);
            visualRoot.localScale =
                Vector3.Lerp(squashedScale, flattenedScale, easedT);
            yield return null;
        }

        deathCoroutine = null;
        gameObject.SetActive(false);
    }
}
