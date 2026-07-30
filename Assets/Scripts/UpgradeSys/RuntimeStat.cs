using UnityEngine;

/// <summary>
/// 업그레이드 효과가 적용된 뒤 수확에서 사용하는 최종 스탯입니다.
/// </summary>
[System.Serializable]
public class RuntimeStat
{
    [Header("수확 스탯")]
    [SerializeField] private float sawSize;
    [SerializeField] private float sawSharpness;
    [SerializeField] private float truckSpeed;
    [SerializeField] private float truckCapacity;

    public float SawSize => Mathf.Max(0f, sawSize);
    public float SawSharpness => Mathf.Max(0f, sawSharpness);
    public float TruckSpeed => Mathf.Max(0f, truckSpeed);
    public float TruckCapacity => Mathf.Max(0f, truckCapacity);

    public void Apply(StatModifier modifier, int level)
    {
        if (modifier == null || level <= 0)
            return;

        float amount = modifier.value * level; 
        switch (modifier.statType)
        {
            case StatType.SawSize:
                sawSize = ApplyValue(sawSize, modifier.modifierType, amount);
                break;
            case StatType.SawSharpness:
                sawSharpness = ApplyValue(sawSharpness, modifier.modifierType, amount);
                break;
            case StatType.TruckSpeed:
                truckSpeed = ApplyValue(truckSpeed, modifier.modifierType, amount);
                break;
            case StatType.TruckCapacity:
                truckCapacity = ApplyValue(truckCapacity, modifier.modifierType, amount);
                break;
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
}
