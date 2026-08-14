using System;
using UnityEngine;

[Serializable]
public sealed class LevelMissionInfo
{
    private enum MissionConditionType
    {
        None = 0,
        DishUpgrade = 1,
        Income = 2,
        FacilityUpgrade = 3,
        EmployeeUpgrade = 4,
        HarvestUpgrade = 5
    }

    [SerializeField] private MissionConditionType conditionType;
    [SerializeReference] private MissionCondition condition;
    [SerializeField] private string title;
    [SerializeField, TextArea] private string description;

    public MissionCondition Condition => condition;
    public string Title => title;
    public string Description => description;

    internal void SyncCondition()
    {
        switch (conditionType)
        {
            case MissionConditionType.None:
                condition = null;
                break;

            case MissionConditionType.DishUpgrade:
                if (condition is not DishUpgradeMissionCondition)
                    condition = new DishUpgradeMissionCondition();
                break;

            case MissionConditionType.Income:
                if (condition is not IncomeMissionCondition)
                    condition = new IncomeMissionCondition();
                break;

            case MissionConditionType.FacilityUpgrade:
                if (condition is not FacilityUpgradeMissionCondition)
                    condition = new FacilityUpgradeMissionCondition();
                break;

            case MissionConditionType.EmployeeUpgrade:
                if (condition is not EmployeeUpgradeMissionCondition)
                    condition = new EmployeeUpgradeMissionCondition();
                break;

            case MissionConditionType.HarvestUpgrade:
                if (condition is not HarvestUpgradeMissionCondition)
                    condition = new HarvestUpgradeMissionCondition();
                break;
        }
    }
}
