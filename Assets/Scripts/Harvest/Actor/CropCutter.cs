using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class CropCutter : MonoBehaviour
{
    private const float CutterOffset = 0.7f;

    [SerializeField] private GridChunkHandler gridChunkHandler;
    [SerializeField] private CutterViewer cutterViewer;
    [SerializeField, Min(0f)] private float cuttingRange = 0.5f;
    [SerializeField, Min(0f)] private float damage = 1f;
    [SerializeField, Min(0.25f)] private float damageDelay = 0.25f;
    [SerializeField, Range(0f, 1f)] private float cuttingMoveSpeedMultiplier = 0.35f;

    private readonly Dictionary<int, float> nextDamageTimes = new();
    private float cuttingUntilTime;

    public bool IsCutting => Time.time <= cuttingUntilTime;
    public float MoveSpeedMultiplier =>
        IsCutting ? cuttingMoveSpeedMultiplier : 1f;

    private void Awake()
    {
        Vector3 localPosition = transform.localPosition;
        localPosition.z = CutterOffset + cuttingRange;
        transform.localPosition = localPosition;

        cutterViewer?.SetRange(cuttingRange);
    }

    private void OnValidate()
    {
        cuttingRange = Mathf.Max(0f, cuttingRange);

        Vector3 localPosition = transform.localPosition;
        localPosition.z = CutterOffset + cuttingRange;
        transform.localPosition = localPosition;

        cutterViewer?.SetRange(cuttingRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            Mathf.Max(0f, cuttingRange));
    }

    private void FixedUpdate()
    {
        if (gridChunkHandler == null)
        {
            return;
        }

        List<Transform> nearbyTransforms =
            gridChunkHandler.Registry.GetNearbyTransforms(
                transform.position,
                cuttingRange);

        foreach (Transform target in nearbyTransforms)
        {
            if (target == null || !target.CompareTag("Harvestable"))
            {
                continue;
            }

            HarvestActor crop = target.GetComponent<HarvestActor>();

            if (crop == null)
            {
                continue;
            }

            cuttingUntilTime = Time.time + Time.fixedDeltaTime * 2f;

            int cropId = crop.GetInstanceID();

            if (nextDamageTimes.TryGetValue(cropId, out float nextDamageTime)
                && Time.time < nextDamageTime)
            {
                continue;
            }

            crop.TakeDamage(damage);
            nextDamageTimes[cropId] =
                Time.time + Mathf.Max(0.25f, damageDelay);
        }
    }
}
