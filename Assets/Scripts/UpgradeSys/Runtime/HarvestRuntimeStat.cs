using System;
using System.Collections.Generic;
using UnityEngine;

public enum HarvestStatType
{
    SawSize,
    SawSpeed,
    SawSharpness,
    TruckSpeed,
    TruckCapacity,
    TruckFuel,
    GoldenPigDetectionRadius,
    Count
}

[Serializable]
public struct HarvestStatModifier
{
    public HarvestStatType statType;
    public ModifierType modifierType;
    public float value;
}

[Serializable]
public struct HarvestStatViewer
{
    [SerializeField] private string statName;
    [SerializeField] private float value;

    public float Value => value;

    internal void SetName(string name)
    {
        statName = name;
    }

    internal void SetValue(float newValue)
    {
        value = newValue;
    }
}

[Serializable]
public sealed class HarvestRuntimeStat
{
    [SerializeField] private HarvestStatViewer[] values = Array.Empty<HarvestStatViewer>();

    public HarvestRuntimeStat()
    {
        EnsureCapacity();
    }

    internal void Initialize()
    {
        EnsureCapacity();

        values[(int)HarvestStatType.SawSize].SetValue(1f);
        values[(int)HarvestStatType.SawSpeed].SetValue(2f);
        values[(int)HarvestStatType.SawSharpness].SetValue(1f);
        values[(int)HarvestStatType.TruckSpeed].SetValue(5f);
        values[(int)HarvestStatType.TruckCapacity].SetValue(10f);
        values[(int)HarvestStatType.TruckFuel].SetValue(40f);
        values[(int)HarvestStatType.GoldenPigDetectionRadius].SetValue(10f);
    }

    public float Get(HarvestStatType statType)
    {
        int index = (int)statType;

        if (!IsValidIndex(index))
            return 0f;

        EnsureCapacity();
        return Mathf.Max(0f, values[index].Value);
    }

    internal void Set(HarvestStatType statType, float value)
    {
        int index = (int)statType;

        if (!IsValidIndex(index))
            return;

        EnsureCapacity();
        values[index].SetValue(value);
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
            values[index].SetValue(ApplyValue(
                values[index].Value,
                modifier.modifierType,
                modifier.value * level));
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
        values ??= Array.Empty<HarvestStatViewer>();

        if (values.Length != (int)HarvestStatType.Count)
            Array.Resize(ref values, (int)HarvestStatType.Count);

        for (int i = 0; i < values.Length; i++)
            values[i].SetName(((HarvestStatType)i).ToString());
    }
}
