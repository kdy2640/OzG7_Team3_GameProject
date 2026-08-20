using System;
using UnityEngine;

[Serializable]
public sealed class MissionGroceryReward : MissionReward
{
    [SerializeField] private GroceryAmount groceryAmount = new();

    public override bool TryGrant()
    {
        StockManager stockManager = GameManager.Instance?.StockManager;

        if (stockManager == null
            || groceryAmount == null
            || (int)groceryAmount.grocery < 0
            || (int)groceryAmount.grocery >= (int)GroceryType.Count
            || groceryAmount.amount <= 0)
        {
            return false;
        }

        stockManager.AddGrocery(new GroceryAmount(
            groceryAmount.grocery,
            groceryAmount.amount));
        return true;
    }

    public override string ToString()
    {
        if (groceryAmount == null)
            return string.Empty;

        return $"{groceryAmount.grocery} x{groceryAmount.amount:N0}";
    }
}
