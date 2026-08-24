using System;
using UnityEngine;

[Serializable]
public sealed class GroceryDeliveryMissionCondition : MissionCondition
{
    [SerializeField] private GroceryAmount groceryAmount = new();

    public override bool IsSatisfied()
    {
        return groceryAmount.amount > 0
            && GameManager.Instance.StockManager.CanConsumeGrocery(groceryAmount);
    }

    internal bool TryDeliver()
    {
        return groceryAmount.amount > 0
            && GameManager.Instance.StockManager.TryConsumeGrocery(groceryAmount);
    }

    public override string ToString()
    {
        long currentAmount = 0;

        foreach (GroceryAmount stockGrocery in
            GameManager.Instance.StockManager.StockData.Groceries)
        {
            if (stockGrocery != null
                && stockGrocery.grocery == groceryAmount.grocery)
            {
                currentAmount += Mathf.Max(0, stockGrocery.amount);
            }
        }

        int clampedCurrentAmount = (int)Math.Min(currentAmount, int.MaxValue);
        return $"{clampedCurrentAmount} / {groceryAmount.amount}";
    }
}
