using System;
using System.Collections.Generic;
using UnityEngine;

public partial class StockManager : MonoBehaviour
{
    [SerializeField] private StockData stockData = new();

    private Action onStockDataChanged;
    private CookingManager cookingManager;

    // UI 등에서 현재 재고를 읽을 때 사용.
    public IReadableStockData StockData => stockData;

    // 요리 제작 로직을 사용할 때 사용.
    public CookingManager CookingManager => cookingManager ??= new CookingManager(
        CalculateGroceryAmount,
        TryConsumeGrocery,
        AddDish);

    #region Currency

    // 재화를 획득했을 때 사용.
    public void AddCurrency(int amount)
    {
        if (!IsValidCurrencyAmount(amount))
        {
            Debug.LogWarning("StockManager.AddCurrency에는 0 이상의 유한한 값만 전달할 수 있습니다.");
            return;
        }

        int addedCurrency = (int)Math.Min((long)stockData.currency + amount, int.MaxValue);
        SetCurrency(addedCurrency);
    }

    // 비용을 지불할 수 있는지 확인할 때 사용.
    public bool CanConsumeCurrency(int amount)
    {
        return IsValidCurrencyAmount(amount) && stockData.currency >= amount;
    }

    // 비용을 확인하고 실제로 지불할 때 사용.
    public bool TryConsumeCurrency(int amount)
    {
        if (!CanConsumeCurrency(amount))
            return false;

        SetCurrency(stockData.currency - amount);
        return true;
    }

    private void SetCurrency(int amount, bool forceNotify = false)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (!forceNotify && stockData.currency == clampedAmount)
            return;

        stockData.currency = clampedAmount;
        NotifyStockDataChanged();
    }

    private static bool IsValidCurrencyAmount(int amount)
    {
        return amount >= 0;
    }

    #endregion

    #region Grocery

    // 식재료 하나를 획득했을 때 사용.
    public void AddGrocery(GroceryAmount groceryAmount)
    {
        AddGrocery(new List<GroceryAmount> { groceryAmount });
    }

    // 식재료 여러 개를 한 번에 획득했을 때 사용.
    public void AddGrocery(List<GroceryAmount> groceryAmounts)
    {
        if (groceryAmounts == null)
        {
            Debug.LogWarning("StockManager.AddGrocery에는 null이 아닌 0 이상의 식재료 수량만 전달할 수 있습니다.");
            return;
        }

        foreach (GroceryAmount groceryAmount in groceryAmounts)
        {
            if (groceryAmount == null || groceryAmount.amount < 0)
            {
                Debug.LogWarning("StockManager.AddGrocery에는 null이 아닌 0 이상의 식재료 수량만 전달할 수 있습니다.");
                return;
            }
        }

        bool hasChanged = false;

        foreach (GroceryAmount groceryAmount in groceryAmounts)
        {
            if (groceryAmount.amount == 0)
                continue;

            GroceryAmount target = stockData.groceries.Find(
                entry => entry != null && entry.grocery == groceryAmount.grocery);

            if (target == null)
            {
                stockData.groceries.Add(new GroceryAmount(
                    groceryAmount.grocery,
                    groceryAmount.amount));
                hasChanged = true;
                continue;
            }

            int addedAmount = (int)Math.Min(
                (long)Mathf.Max(0, target.amount) + groceryAmount.amount,
                int.MaxValue);

            if (target.amount == addedAmount)
                continue;

            target.amount = addedAmount;
            hasChanged = true;
        }

        if (hasChanged)
            NotifyStockDataChanged();
    }

    // 식재료 하나를 사용할 수 있는지 확인할 때 사용.
    public bool CanConsumeGrocery(GroceryAmount groceryAmount)
    {
        return CanConsumeGrocery(new List<GroceryAmount> { groceryAmount });
    }

    // 여러 식재료를 모두 사용할 수 있는지 확인할 때 사용.
    public bool CanConsumeGrocery(List<GroceryAmount> groceryAmounts)
    {
        if (groceryAmounts == null)
            return false;

        foreach (GroceryAmount groceryAmount in groceryAmounts)
        {
            if (groceryAmount == null || groceryAmount.amount < 0)
                return false;

            long requiredAmount = 0;

            foreach (GroceryAmount requestedAmount in groceryAmounts)
            {
                if (requestedAmount != null
                    && requestedAmount.grocery == groceryAmount.grocery)
                {
                    requiredAmount += requestedAmount.amount;
                }
            }

            if (CalculateGroceryAmount(groceryAmount.grocery) < requiredAmount)
                return false;
        }

        return true;
    }

    // 식재료 하나를 확인하고 실제로 사용할 때 사용.
    public bool TryConsumeGrocery(GroceryAmount groceryAmount)
    {
        return TryConsumeGrocery(new List<GroceryAmount> { groceryAmount });
    }

    // 여러 식재료를 확인하고 한 번에 사용할 때 사용.
    public bool TryConsumeGrocery(List<GroceryAmount> groceryAmounts)
    {
        if (!CanConsumeGrocery(groceryAmounts))
            return false;

        bool hasChanged = false;

        foreach (GroceryAmount groceryAmount in groceryAmounts)
        {
            int remainingAmount = groceryAmount.amount;

            if (remainingAmount == 0)
                continue;

            hasChanged = true;

            foreach (GroceryAmount stockGroceryAmount in stockData.groceries)
            {
                if (remainingAmount == 0)
                    break;

                if (stockGroceryAmount == null
                    || stockGroceryAmount.grocery != groceryAmount.grocery)
                    continue;

                int consumableAmount = Mathf.Min(
                    Mathf.Max(0, stockGroceryAmount.amount),
                    remainingAmount);
                stockGroceryAmount.amount -= consumableAmount;
                remainingAmount -= consumableAmount;
            }
        }

        if (hasChanged)
            NotifyStockDataChanged();

        return true;
    }

    private int CalculateGroceryAmount(GroceryType grocery)
    {
        long total = 0;

        foreach (GroceryAmount groceryAmount in stockData.groceries)
        {
            if (groceryAmount != null && groceryAmount.grocery == grocery)
                total += Mathf.Max(0, groceryAmount.amount);
        }

        return (int)Math.Min(total, int.MaxValue);
    }

    #endregion

    #region Dish

    // 요리 하나를 획득했을 때 사용.
    private void AddDish(DishAmount dishAmount)
    {
        AddDish(new List<DishAmount> { dishAmount });
    }

    // 요리 여러 개를 한 번에 획득했을 때 사용.
    private void AddDish(List<DishAmount> dishAmounts)
    {
        if (dishAmounts == null)
        {
            Debug.LogWarning("StockManager.AddDish에는 null이 아닌 0 이상의 요리 수량만 전달할 수 있습니다.");
            return;
        }

        foreach (DishAmount dishAmount in dishAmounts)
        {
            if (dishAmount == null || dishAmount.amount < 0)
            {
                Debug.LogWarning("StockManager.AddDish에는 null이 아닌 0 이상의 요리 수량만 전달할 수 있습니다.");
                return;
            }
        }

        bool hasChanged = false;

        foreach (DishAmount dishAmount in dishAmounts)
        {
            if (dishAmount.amount == 0)
                continue;

            DishAmount target = stockData.dishes.Find(
                entry => entry != null && entry.dish == dishAmount.dish);

            if (target == null)
            {
                stockData.dishes.Add(new DishAmount(
                    dishAmount.dish,
                    dishAmount.amount));
                hasChanged = true;
                continue;
            }

            int addedAmount = (int)Math.Min(
                (long)Mathf.Max(0, target.amount) + dishAmount.amount,
                int.MaxValue);

            if (target.amount == addedAmount)
                continue;

            target.amount = addedAmount;
            hasChanged = true;
        }

        if (hasChanged)
            NotifyStockDataChanged();
    }

    // 요리 하나를 사용할 수 있는지 확인할 때 사용.
    public bool CanConsumeDish(DishAmount dishAmount)
    {
        return CanConsumeDish(new List<DishAmount> { dishAmount });
    }

    // 여러 요리를 모두 사용할 수 있는지 확인할 때 사용.
    public bool CanConsumeDish(List<DishAmount> dishAmounts)
    {
        if (dishAmounts == null)
            return false;

        foreach (DishAmount dishAmount in dishAmounts)
        {
            if (dishAmount == null || dishAmount.amount < 0)
                return false;

            long requiredAmount = 0;

            foreach (DishAmount requestedAmount in dishAmounts)
            {
                if (requestedAmount != null
                    && requestedAmount.dish == dishAmount.dish)
                {
                    requiredAmount += requestedAmount.amount;
                }
            }

            if (CalculateDishAmount(dishAmount.dish) < requiredAmount)
                return false;
        }

        return true;
    }

    // 요리 하나를 확인하고 실제로 사용할 때 사용.
    public bool TryConsumeDish(DishAmount dishAmount)
    {
        return TryConsumeDish(new List<DishAmount> { dishAmount });
    }

    // 여러 요리를 확인하고 한 번에 사용할 때 사용.
    public bool TryConsumeDish(List<DishAmount> dishAmounts)
    {
        if (!CanConsumeDish(dishAmounts))
            return false;

        bool hasChanged = false;

        foreach (DishAmount dishAmount in dishAmounts)
        {
            int remainingAmount = dishAmount.amount;

            if (remainingAmount == 0)
                continue;

            hasChanged = true;

            foreach (DishAmount stockDishAmount in stockData.dishes)
            {
                if (remainingAmount == 0)
                    break;

                if (stockDishAmount == null
                    || stockDishAmount.dish != dishAmount.dish)
                    continue;

                int consumableAmount = Mathf.Min(
                    Mathf.Max(0, stockDishAmount.amount),
                    remainingAmount);
                stockDishAmount.amount -= consumableAmount;
                remainingAmount -= consumableAmount;
            }
        }

        if (hasChanged)
            NotifyStockDataChanged();

        return true;
    }

    private int CalculateDishAmount(DishType dish)
    {
        long total = 0;

        foreach (DishAmount dishAmount in stockData.dishes)
        {
            if (dishAmount != null && dishAmount.dish == dish)
                total += Mathf.Max(0, dishAmount.amount);
        }

        return (int)Math.Min(total, int.MaxValue);
    }

    // 전체 요리 없앨때 사용
    public void ClearDishes()
    {
        foreach (DishAmount dishAmount in stockData.dishes)
        {
            dishAmount.amount = 0;
        }

        NotifyStockDataChanged();
    }

    #endregion

    #region Save Data

    public StockSaveData CreateStockSaveData()
    {
        StockSaveData saveData = new()
        {
            currency = stockData.currency
        };

        foreach (GroceryAmount groceryAmount in stockData.groceries)
        {
            if (groceryAmount == null)
                continue;

            saveData.groceries.Add(new GroceryAmount(
                groceryAmount.grocery,
                groceryAmount.amount));
        }

        foreach (DishAmount dishAmount in stockData.dishes)
        {
            if (dishAmount == null)
                continue;

            saveData.dishes.Add(new DishAmount(
                dishAmount.dish,
                dishAmount.amount));
        }

        return saveData;
    }

    public void LoadStockSaveData(StockSaveData saveData)
    {
        stockData = new StockData();

        if (saveData != null)
        {
            if (IsValidCurrencyAmount(saveData.currency))
                stockData.currency = saveData.currency;

            if (saveData.groceries != null)
            {
                foreach (GroceryAmount groceryAmount in saveData.groceries)
                {
                    if (groceryAmount == null
                        || (int)groceryAmount.grocery < 0
                        || (int)groceryAmount.grocery >= (int)GroceryType.Count)
                        continue;

                    stockData.groceries.Add(new GroceryAmount(
                        groceryAmount.grocery,
                        Mathf.Max(0, groceryAmount.amount)));
                }
            }

            if (saveData.dishes != null)
            {
                foreach (DishAmount dishAmount in saveData.dishes)
                {
                    if (dishAmount == null
                        || (int)dishAmount.dish < 0
                        || (int)dishAmount.dish >= (int)DishType.Count)
                        continue;

                    stockData.dishes.Add(new DishAmount(
                        dishAmount.dish,
                        Mathf.Max(0, dishAmount.amount)));
                }
            }
        }

        NotifyStockDataChanged();
    }

    public void ResetStockSaveData()
    {
        stockData = new StockData();
        NotifyStockDataChanged();
    }

    #endregion

    #region Stock Data Change

    // 재고가 바뀔 때 갱신이 필요한 객체가 사용.
    public void SubscribeStockDataChange(Action callback)
    {
        onStockDataChanged += callback;
    }

    // 재고 변경 알림이 더 이상 필요하지 않을 때 사용.
    public void UnsubscribeStockDataChange(Action callback)
    {
        onStockDataChanged -= callback;
    }

    private void NotifyStockDataChanged()
    {
        onStockDataChanged?.Invoke();
    }

    #endregion
}
