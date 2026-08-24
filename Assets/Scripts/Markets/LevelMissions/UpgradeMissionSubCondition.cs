using System;
using UnityEngine;

[Serializable]
public abstract class UpgradeMissionSubCondition
{
    public abstract bool IsSatisfied();
}

[Serializable]
public sealed class UpgradeMissionSubConditionEntry
{
    private enum UpgradeMissionSubConditionType
    {
        Dish = 0,
        Facility = 1,
        Employee = 2,
        Harvest = 3
    }

    [SerializeField] private UpgradeMissionSubConditionType type;
    [SerializeReference] private UpgradeMissionSubCondition subCondition;

    internal UpgradeMissionSubCondition SubCondition => subCondition;

    internal void SyncSubCondition()
    {
        switch (type)
        {
            case UpgradeMissionSubConditionType.Dish:
                if (subCondition is not DishUpgradeMissionSubCondition)
                    subCondition = new DishUpgradeMissionSubCondition();
                break;

            case UpgradeMissionSubConditionType.Facility:
                if (subCondition is not FacilityUpgradeMissionSubCondition)
                    subCondition = new FacilityUpgradeMissionSubCondition();
                break;

            case UpgradeMissionSubConditionType.Employee:
                if (subCondition is not EmployeeUpgradeMissionSubCondition)
                    subCondition = new EmployeeUpgradeMissionSubCondition();
                break;

            case UpgradeMissionSubConditionType.Harvest:
                if (subCondition is not HarvestUpgradeMissionSubCondition)
                    subCondition = new HarvestUpgradeMissionSubCondition();
                break;
        }
    }
}
