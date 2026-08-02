using System;
using System.Collections.Generic;
using UnityEngine;

public enum HarvestStatType
{
    SawSize,
    SawSharpness,
    TruckSpeed,
    TruckCapacity,
    Count
}

public enum ModifierType
{
    Add,
    Multiply,
    Max
}

[Serializable]
public struct HarvestStatModifier
{
    public HarvestStatType statType;
    public ModifierType modifierType;
    public float value;
}

[Serializable]
public sealed class HarvestRuntimeStat
{
    [SerializeField] private float[] values = Array.Empty<float>();

    public float Get(HarvestStatType statType)
    {
        int index = (int)statType;

        if (!IsValidIndex(index))
            return 0f;

        EnsureCapacity();
        return Mathf.Max(0f, values[index]);
    }

    internal void Apply(
        IReadOnlyList<HarvestStatModifier> modifiers,
        int level)
    {
        if (modifiers == null || level <= 0)
            return;

        for (int i = 0; i < modifiers.Count; i++)
        {
            HarvestStatModifier modifier = modifiers[i];
            int index = (int)modifier.statType;

            if (!IsValidIndex(index))
                continue;

            EnsureCapacity();
            values[index] = ApplyValue(
                values[index],
                modifier.modifierType,
                modifier.value * level);
        }
    }

    private static float ApplyValue(float current, ModifierType type, float amount)
    {
        return type switch
        {
            ModifierType.Add => current + amount,
            ModifierType.Multiply => current * (1f + amount),
            ModifierType.Max => Mathf.Max(current, amount),
            _ => current
        };
    }

    private static bool IsValidIndex(int index)
    {
        return index >= 0 && index < (int)HarvestStatType.Count;
    }

    private void EnsureCapacity()
    {
        values ??= Array.Empty<float>();

        if (values.Length != (int)HarvestStatType.Count)
            Array.Resize(ref values, (int)HarvestStatType.Count);
    }
}
