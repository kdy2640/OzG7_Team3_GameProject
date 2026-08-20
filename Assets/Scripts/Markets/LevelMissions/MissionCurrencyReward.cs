using System;
using UnityEngine;

[Serializable]
public sealed class MissionCurrencyReward : MissionReward
{
    [SerializeField, Min(1)] private int amount = 1;

    public override bool TryGrant()
    {
        StockManager stockManager = GameManager.Instance?.StockManager;

        if (stockManager == null || amount <= 0)
            return false;

        stockManager.AddCurrency(amount);
        return true;
    }

    public override string ToString()
    {
        return $"{amount:N0} Currency";
    }
}
