using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Harvest")]
public sealed class HarvestUpgradeDataSO : UpgradeDataSO
{
    [SerializeField] private List<HarvestStatModifier> statModifiers = new();

    public List<HarvestStatModifier> StatModifiers => statModifiers;

    public override void ApplyTo(RuntimeStat runtimeStat, int level)
    {
        if (runtimeStat == null)
            return;

        runtimeStat.Harvest.Apply(statModifiers, level);
    }
}
