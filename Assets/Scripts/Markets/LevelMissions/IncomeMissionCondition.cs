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

        return GameManager.Instance.Market.MarketData.TotalIncome >= targetIncome;
    }
}
