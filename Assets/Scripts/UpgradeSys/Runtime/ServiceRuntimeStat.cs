using System;
using System.Collections.Generic;
using UnityEngine;

public enum ServiceStatType
{
    CustomerCount,
    ExtraTime,
    TipMultiplier,
    Count
}

[Serializable]
public struct ServiceStatModifier
{
    public ServiceStatType statType;
    public ModifierType modifierType;
    public float value;
}

[Serializable]
public struct ServiceStatViewer
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
public sealed class ServiceRuntimeStat
{
    [SerializeField] private ServiceStatViewer[] values = Array.Empty<ServiceStatViewer>();

    public ServiceRuntimeStat()
    {
        EnsureCapacity();
    }

    public float Get(ServiceStatType statType)
    {
        int index = (int)statType;

        if (!IsValidIndex(index))
            return 0f;

        EnsureCapacity();
        return Mathf.Max(0f, values[index].Value);
    }

    internal void Apply(
        IReadOnlyList<ServiceStatModifier> modifiers,
        int level)
    {
        if (modifiers == null || level <= 0)
            return;

        for (int i = 0; i < modifiers.Count; i++)
        {
            ServiceStatModifier modifier = modifiers[i];
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
        return index >= 0 && index < (int)ServiceStatType.Count;
    }

    private void EnsureCapacity()
    {
        values ??= Array.Empty<ServiceStatViewer>();

        if (values.Length != (int)ServiceStatType.Count)
            Array.Resize(ref values, (int)ServiceStatType.Count);

        for (int i = 0; i < values.Length; i++)
            values[i].SetName(((ServiceStatType)i).ToString());
    }
}
