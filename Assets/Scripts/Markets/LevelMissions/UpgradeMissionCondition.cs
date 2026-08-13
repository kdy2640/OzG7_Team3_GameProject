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

        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType) >= targetLevel;
    }

    public override string ToString()
    {
        int currentLevel = GameManager.Instance?.Upgrade?.RuntimeLevel.Get(dishType) ?? 0;
        return $"{currentLevel} / {targetLevel}";
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

        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(facilityType) >= targetLevel;
    }

    public override string ToString()
    {
        int currentLevel = GameManager.Instance?.Upgrade?.RuntimeLevel.Get(facilityType) ?? 0;
        return $"{currentLevel} / {targetLevel}";
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

        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(employeeType) >= targetLevel;
    }

    public override string ToString()
    {
        int currentLevel = GameManager.Instance?.Upgrade?.RuntimeLevel.Get(employeeType) ?? 0;
        return $"{currentLevel} / {targetLevel}";
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

        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(harvestUpgradeType) >= targetLevel;
    }

    public override string ToString()
    {
        int currentLevel = GameManager.Instance?.Upgrade?.RuntimeLevel.Get(harvestUpgradeType) ?? 0;
        return $"{currentLevel} / {targetLevel}";
    }
}
