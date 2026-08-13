using System;
using UnityEngine;

[Serializable]
public sealed class IncomeMissionCondition : MissionCondition
{
    [SerializeField, Min(0)] private int targetIncome;

    public override bool IsSatisfied()
    {
        if (GameManager.Instance == null || GameManager.Instance.Market == null)
            return false;

        return targetIncome > 0
            && GameManager.Instance.Market.MarketData.TotalIncome >= targetIncome;
    }

    public override string ToString()
    {
        int totalIncome = GameManager.Instance?.Market?.MarketData?.TotalIncome ?? 0;
        return $"{totalIncome:N0} / {targetIncome:N0}";
    }
}
