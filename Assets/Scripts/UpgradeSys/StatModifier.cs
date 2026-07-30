using System;

public enum StatType
{
    SawSize,
    SawSharpness,
    TruckSpeed,
    TruckCapacity
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
