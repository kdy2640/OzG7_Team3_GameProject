using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class CropCutter : MonoBehaviour
{
    [SerializeField, Min(0f)] private float damage = 1f;
    [SerializeField, Min(0.25f)] private float damageDelay = 0.25f;
    [SerializeField, Range(0f, 1f)] private float cuttingMoveSpeedMultiplier = 0.35f;

    private readonly Dictionary<int, float> nextDamageTimes = new();
    private float cuttingUntilTime;

    public bool IsCutting => Time.time <= cuttingUntilTime;
    public float MoveSpeedMultiplier =>
        IsCutting ? cuttingMoveSpeedMultiplier : 1f;

    private void OnTriggerStay(Collider other)
    {
        HarvestActor crop = other.GetComponentInParent<HarvestActor>();

        if (crop == null)
        {
            return;
        }

        cuttingUntilTime = Time.time + Time.fixedDeltaTime * 2f;

        int cropId = crop.GetInstanceID();

        if (nextDamageTimes.TryGetValue(cropId, out float nextDamageTime)
            && Time.time < nextDamageTime)
        {
            return;
        }

        crop.TakeDamage(damage);
        nextDamageTimes[cropId] =
            Time.time + Mathf.Max(0.25f, damageDelay);
    }
}
