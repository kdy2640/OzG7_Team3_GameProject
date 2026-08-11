using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class MarketData
{
    [SerializeField, Min(0)] private int currentBusinessDay;
    [SerializeField, Min(0)] private int currentLevel;
    [FormerlySerializedAs("currentEXP")]
    [SerializeField, Min(0)] private int totalIncome;
    [SerializeField] private List<DishType> selectedDishes = new();

    internal event Action OnMarketDataChanged;

    public int CurrentBusinessDay
    {
        get => currentBusinessDay;
        set
        {
            int nextBusinessDay = Mathf.Max(0, value);

            if (currentBusinessDay == nextBusinessDay)
                return;

            currentBusinessDay = nextBusinessDay;
            NotifyMarketDataChanged();
        }
    }

    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            int nextLevel = Mathf.Max(0, value);

            if (currentLevel == nextLevel)
                return;

            currentLevel = nextLevel;
            NotifyMarketDataChanged();
        }
    }

    public int TotalIncome
    {
        get => totalIncome;
        set
        {
            int nextTotalIncome = Mathf.Max(0, value);

            if (totalIncome == nextTotalIncome)
                return;

            totalIncome = nextTotalIncome;
            NotifyMarketDataChanged();
        }
    }

    public IReadOnlyList<DishType> SelectedDishes => selectedDishes;

    public MarketData()
    {
    }

    internal MarketData(
        int currentBusinessDay,
        int currentLevel,
        int totalIncome,
        List<DishType> selectedDishes)
    {
        this.currentBusinessDay = currentBusinessDay;
        this.currentLevel = currentLevel;
        this.totalIncome = totalIncome;
        this.selectedDishes = selectedDishes == null
            ? new List<DishType>()
            : new List<DishType>(selectedDishes);
    }

    public bool SelectDish(DishType dishType)
    {
        if (selectedDishes.Contains(dishType))
            return false;

        selectedDishes.Add(dishType);
        NotifyMarketDataChanged();
        return true;
    }

    public bool DeselectDish(DishType dishType)
    {
        if (!selectedDishes.Remove(dishType))
            return false;

        NotifyMarketDataChanged();
        return true;
    }

    private void NotifyMarketDataChanged()
    {
        OnMarketDataChanged?.Invoke();
    }
}
