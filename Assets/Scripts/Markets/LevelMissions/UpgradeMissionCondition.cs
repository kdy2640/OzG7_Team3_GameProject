using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class UpgradeMissionCondition : MissionCondition
{
    [SerializeField, Min(1)] private int requiredSatisfiedCount = 1;
    [SerializeField] private List<UpgradeMissionSubConditionEntry> subConditions = new();

    public override bool IsSatisfied()
    {
        return requiredSatisfiedCount > 0
            && CountSatisfiedSubConditions() >= requiredSatisfiedCount;
    }

    public override string ToString()
    {
        int satisfiedCount = Mathf.Min(
            CountSatisfiedSubConditions(),
            requiredSatisfiedCount);
        return $"{satisfiedCount} / {requiredSatisfiedCount}";
    }

    internal void SyncSubConditions()
    {
        for (int i = 0; i < subConditions.Count; i++)
            subConditions[i].SyncSubCondition();
    }

    private int CountSatisfiedSubConditions()
    {
        int satisfiedCount = 0;

        for (int i = 0; i < subConditions.Count; i++)
        {
            if (subConditions[i].SubCondition.IsSatisfied())
                satisfiedCount++;
        }

        return satisfiedCount;
    }
}

[Serializable]
public sealed class DishUpgradeMissionSubCondition : UpgradeMissionSubCondition
{
    [SerializeField] private DishType dishType = DishType.None;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(dishType) >= targetLevel;
    }
}

[Serializable]
public sealed class FacilityUpgradeMissionSubCondition : UpgradeMissionSubCondition
{
    [SerializeField] private FacilityType facilityType = FacilityType.Count;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(facilityType) >= targetLevel;
    }
}

[Serializable]
public sealed class EmployeeUpgradeMissionSubCondition : UpgradeMissionSubCondition
{
    [SerializeField] private EmployeeType employeeType = EmployeeType.Count;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(employeeType) >= targetLevel;
    }
}

[Serializable]
public sealed class HarvestUpgradeMissionSubCondition : UpgradeMissionSubCondition
{
    [SerializeField] private HarvestUpgradeType harvestUpgradeType = HarvestUpgradeType.Count;
    [SerializeField, Min(1)] private int targetLevel = 1;

    public override bool IsSatisfied()
    {
        return targetLevel > 0
            && GameManager.Instance.Upgrade.RuntimeLevel.Get(harvestUpgradeType) >= targetLevel;
    }
}
