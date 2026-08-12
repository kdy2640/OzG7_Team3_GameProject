using System;
using UnityEngine;

[Serializable]
public sealed class DishUpgradeMissionCondition : MissionCondition
{
    [SerializeField] private DishType dishType = DishType.None;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
            return false;

        return GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType) >= targetLevel;
    }
}

[Serializable]
public sealed class FacilityUpgradeMissionCondition : MissionCondition
{
    [SerializeField] private FacilityType facilityType = FacilityType.Count;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
            return false;

        return GameManager.Instance.Upgrade.RuntimeLevel.Get(facilityType) >= targetLevel;
    }
}

[Serializable]
public sealed class EmployeeUpgradeMissionCondition : MissionCondition
{
    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
            return false;

        return GameManager.Instance.Upgrade.RuntimeLevel.Get(employeeType) >= targetLevel;
    }
}

[Serializable]
public sealed class HarvestUpgradeMissionCondition : MissionCondition
{
    [SerializeField] private HarvestUpgradeType harvestUpgradeType = HarvestUpgradeType.Count;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
            return false;

        return GameManager.Instance.Upgrade.RuntimeLevel.Get(harvestUpgradeType) >= targetLevel;
    }
}
