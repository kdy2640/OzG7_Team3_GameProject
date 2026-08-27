using System.Collections.Generic;
using UnityEngine;

public enum HarvestUpgradeType
{
    SawSize, 
    SawSpeed,
    SawSharpness,
    TruckSpeed,
    TruckCapacity,
    TruckFuel,
    StageLevel,
    GoldenPigRadar,
    Count
}

[CreateAssetMenu(menuName = "Game/Upgrade/Harvest")]
public sealed class HarvestUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private HarvestUpgradeType targetUpgrade = HarvestUpgradeType.Count;
    [SerializeField] private List<HarvestStatModifier> statModifiers = new();

    public HarvestUpgradeType TargetUpgrade => targetUpgrade;
    public IReadOnlyList<HarvestStatModifier> StatModifiers => statModifiers;
}
