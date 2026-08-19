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
    [SerializeField, Min(0f)] private float rangeLerpSpeed = 8f;
    [SerializeField, Min(0f)] private float damage = 1f;
    [SerializeField, Min(0f)] private float damageDelay = 0.25f;
    [SerializeField, Range(0f, 1f)] private float cuttingMoveSpeedMultiplier = 0.35f;

    private readonly Dictionary<int, float> nextDamageTimes = new();
    private float cuttingUntilTime;
    private float damageMultiplier = 1f;

    public bool IsCutting => Time.time <= cuttingUntilTime;
    public float Range => cuttingRange;
    public float TargetRange { get; private set; }
    public float MoveSpeedMultiplier =>
        IsCutting ? cuttingMoveSpeedMultiplier : 1f;

    public void Initialize(GridChunkHandler handler)
    {
        gridChunkHandler = handler;
    }

    public void ApplyUpgradeStats(
        float range,
        float attacksPerSecond,
        float baseDamage)
    {
        TargetRange = Mathf.Max(0f, range);
        ApplyRange(TargetRange);

        damageDelay = attacksPerSecond > 0f
            ? 1f / attacksPerSecond
            : float.MaxValue;
        damage = Mathf.Max(0f, baseDamage);
    }

    private void Awake()
    {
        TargetRange = cuttingRange;
        ApplyRange(cuttingRange);
    }

    private void Update()
    {
        if (Mathf.Approximately(cuttingRange, TargetRange))
        {
            return;
        }

        float nextRange = Mathf.Lerp(
            cuttingRange,
            TargetRange,
            rangeLerpSpeed * Time.deltaTime);

        if (Mathf.Abs(nextRange - TargetRange) < 0.001f)
        {
            nextRange = TargetRange;
        }

        ApplyRange(nextRange);
    }

    private void OnValidate()
    {
        cuttingRange = Mathf.Max(0f, cuttingRange);
        rangeLerpSpeed = Mathf.Max(0f, rangeLerpSpeed);
        TargetRange = cuttingRange;

        ApplyRange(cuttingRange);
    }

    public void SetTargetRange(float range)
    {
        TargetRange = Mathf.Max(0f, range);
    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = Mathf.Max(0f, multiplier);
    }

    private void ApplyRange(float range)
    {
        cuttingRange = Mathf.Max(0f, range);

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
        if (GameManager.Instance?.Harvest?.IsRunning != true)
            return;

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

            crop.TakeDamage(damage * damageMultiplier);
            nextDamageTimes[cropId] =
                Time.time + Mathf.Max(0f, damageDelay);
        }
    }
}
