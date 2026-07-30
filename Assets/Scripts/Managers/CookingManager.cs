using System;
using System.Collections.Generic;

public sealed class CookingManager
{
    private readonly Func<List<GroceryAmount>, bool> canConsumeGrocery;
    private readonly Func<List<GroceryAmount>, bool> tryConsumeGrocery;
    private readonly Action<DishAmount> addDish;

    internal CookingManager(
        Func<List<GroceryAmount>, bool> canConsumeGrocery,
        Func<List<GroceryAmount>, bool> tryConsumeGrocery,
        Action<DishAmount> addDish)
    {
        this.canConsumeGrocery = canConsumeGrocery;
        this.tryConsumeGrocery = tryConsumeGrocery;
        this.addDish = addDish;
    }

    // 해당 요리를 만들 수 있는지 확인할 때 사용.
    public bool CanCook(DishType dish)
    {
        if (dish == DishType.Count
            || !DishDataDB.TryGetData(dish, out DishDataSO data))
            return false;

        return canConsumeGrocery(data.ingredients);
    }

    // 재료를 소모하고 해당 요리를 하나 만들 때 사용.
    public bool TryCook(DishType dish)
    {
        if (dish == DishType.Count
            || !DishDataDB.TryGetData(dish, out DishDataSO data))
            return false;

        if (!tryConsumeGrocery(data.ingredients))
            return false;

        addDish(new DishAmount(dish, 1));
        return true;
    }
}
