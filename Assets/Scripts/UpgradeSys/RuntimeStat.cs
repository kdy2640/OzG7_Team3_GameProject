using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 업그레이드 효과가 적용된 뒤 게임에서 사용하는 최종 스탯입니다.
/// </summary>
[System.Serializable]
public class RuntimeStat
{
    [Header("화살 스탯")]
    [SerializeField] private float arrowDamage;
    [SerializeField] private float arrowFrequency;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float arrowRange;
    [SerializeField] private float arrowCount;
    
    public int ArrowDamage => Mathf.Max(0, Mathf.RoundToInt(arrowDamage)); 
    public int ArrowFrequency => Mathf.Max(0, Mathf.RoundToInt(arrowFrequency));

    public int ArrowSpeed => Mathf.Max(0, Mathf.RoundToInt(arrowSpeed));
    public int ArrowRange => Mathf.Max(0, Mathf.RoundToInt(arrowRange));
    public int ArrowCount => Mathf.Max(0, Mathf.RoundToInt(arrowCount));

     

    public void Apply(StatModifier modifier, int level)
    {
        if (modifier == null || level <= 0)
            return;

        float amount = modifier.value * level; 
        switch (modifier.statType)
        {
            case StatType.ArrowDamage:
                arrowDamage = ApplyValue(arrowDamage, modifier.modifierType, amount);
                break;
            case StatType.ArrowFrequency:
                arrowFrequency = ApplyValue(arrowFrequency, modifier.modifierType, amount);
                break;
            case StatType.ArrowSpeed:
                arrowSpeed = ApplyValue(arrowSpeed, modifier.modifierType, amount);
                break;
            case StatType.ArrowRange:
                arrowRange = ApplyValue(arrowRange, modifier.modifierType, amount);
                break;
            case StatType.ArrowCount:
                arrowCount = ApplyValue(arrowCount, modifier.modifierType, amount);
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
