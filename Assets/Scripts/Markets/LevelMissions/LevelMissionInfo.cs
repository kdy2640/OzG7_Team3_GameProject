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

    private enum MissionRewardType
    {
        None = 0,
        Currency = 1,
        Grocery = 2
    }

    [SerializeField] private MissionConditionType conditionType;
    [SerializeReference] private MissionCondition condition;
    [SerializeField] private MissionRewardType rewardType;
    [SerializeReference] private MissionReward reward;
    [SerializeField] private string title;
    [SerializeField, TextArea] private string description;

    public MissionCondition Condition => condition;
    public MissionReward Reward => reward;
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

    internal void SyncReward()
    {
        switch (rewardType)
        {
            case MissionRewardType.None:
                reward = null;
                break;

            case MissionRewardType.Currency:
                if (reward is not MissionCurrencyReward)
                    reward = new MissionCurrencyReward();
                break;

            case MissionRewardType.Grocery:
                if (reward is not MissionGroceryReward)
                    reward = new MissionGroceryReward();
                break;
        }
    }
}
