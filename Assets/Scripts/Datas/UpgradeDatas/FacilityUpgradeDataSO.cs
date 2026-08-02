using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Facility")]
public sealed class FacilityUpgradeDataSO : UpgradeDataSO
{
    public FacilityType targetFacility = FacilityType.Count;

    public override void ApplyTo(RuntimeStat runtimeStat, int level)
    {
        if (runtimeStat == null)
            return;

        runtimeStat.Facility.Apply(targetFacility, level);
    }
}
