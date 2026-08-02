using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Harvest")]
public sealed class HarvestUpgradeDataSO : UpgradeDataSO
{
    public List<HarvestStatModifier> statModifiers = new();

    public override void ApplyTo(RuntimeStat runtimeStat, int level)
    {
        if (runtimeStat == null)
            return;

        runtimeStat.Harvest.Apply(statModifiers, level);
    }
}
