using System;

public enum StatType
{
    ArrowDamage,
    ArrowFrequency,
    ArrowSpeed,
    ArrowRange,
    ArrowCount
}

public enum ModifierType
{
    Add,
    Multiply,
    Max
}

[Serializable]
public class StatModifier
{
    public StatType statType;
    public ModifierType modifierType;
    public float value;
}
