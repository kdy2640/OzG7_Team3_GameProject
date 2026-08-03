using System;
using System.Collections.Generic;

public sealed class CookingManager
{
    private readonly Func<GroceryType, int> calculateGroceryAmount;
    private readonly Func<List<GroceryAmount>, bool> tryConsumeGrocery;
    private readonly Action<DishAmount> addDish;

    internal CookingManager(
        Func<GroceryType, int> calculateGroceryAmount,
        Func<List<GroceryAmount>, bool> tryConsumeGrocery,
        Action<DishAmount> addDish)
    {
        this.calculateGroceryAmount = calculateGroceryAmount;
        this.tryConsumeGrocery = tryConsumeGrocery;
        this.addDish = addDish;
    }

    // 해당 요리를 만들 수 있는지 확인할 때 사용.
    public bool CanCook(DishType dish)
    {
        return CalculateCookableAmount(dish) > 0;
    }

    // 현재 식재료 재고로 만들 수 있는 해당 요리의 최대 수량을 계산할 때 사용.
    public int CalculateCookableAmount(DishType dish)
    {
        if (dish == DishType.Count
            || !DishDataDB.TryGetData(dish, out DishDataSO data)
            || data.Ingredients == null)
            return 0;

        Dictionary<GroceryType, long> requiredAmounts = new();

        foreach (GroceryAmount ingredient in data.Ingredients)
        {
            if (ingredient == null
                || ingredient.grocery == GroceryType.Count
                || ingredient.amount < 0)
                return 0;

            if (ingredient.amount == 0)
                continue;

            if (!requiredAmounts.TryAdd(
                    ingredient.grocery,
                    ingredient.amount))
            {
                requiredAmounts[ingredient.grocery] += ingredient.amount;
            }
        }

        if (requiredAmounts.Count == 0)
            return 0;

        int cookableAmount = int.MaxValue;

        foreach (KeyValuePair<GroceryType, long> requiredAmount in requiredAmounts)
        {
            int groceryAmount = calculateGroceryAmount(requiredAmount.Key);
            int amount = (int)(groceryAmount / requiredAmount.Value);
            cookableAmount = Math.Min(cookableAmount, amount);
        }

        return cookableAmount;
    }

    // 재료를 소모하고 해당 요리를 하나 만들 때 사용.
    public bool TryCook(DishType dish)
    {
        if (dish == DishType.Count
            || !DishDataDB.TryGetData(dish, out DishDataSO data))
            return false;

        if (!tryConsumeGrocery(data.Ingredients))
            return false;

        addDish(new DishAmount(dish, 1));
        return true;
    }
}
