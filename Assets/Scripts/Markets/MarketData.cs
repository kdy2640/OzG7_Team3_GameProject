using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MarketPhase
{
    Morning,
    Afternoon,
    Night
}

[Serializable]
public sealed class MarketData
{
    [SerializeField, Min(0)] internal int currentBusinessDay;
    [SerializeField] internal MarketPhase currentPhase = MarketPhase.Morning;
    [SerializeField, Min(0)] internal int currentLevel;
    [FormerlySerializedAs("currentEXP")]
    [SerializeField, Min(0)] internal int totalIncome;
    [SerializeField, Min(0)] internal int yesterdaySales;
    [SerializeField] internal List<DishType> selectedDishes = new();

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

    public MarketPhase CurrentPhase
    {
        get => currentPhase;
        set
        {
            if (currentPhase == value)
                return;

            currentPhase = value;
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

    public int YesterdaySales
    {
        get => yesterdaySales;
        set
        {
            int nextYesterdaySales = Mathf.Max(0, value);

            if (yesterdaySales == nextYesterdaySales)
                return;

            yesterdaySales = nextYesterdaySales;
            NotifyMarketDataChanged();
        }
    }

    public IReadOnlyList<DishType> SelectedDishes => selectedDishes;

    public MarketData()
    {
    }

    internal MarketData(
        int currentBusinessDay,
        MarketPhase currentPhase,
        int currentLevel,
        int totalIncome,
        int yesterdaySales,
        List<DishType> selectedDishes)
    {
        this.currentBusinessDay = currentBusinessDay;
        this.currentPhase = currentPhase;
        this.currentLevel = currentLevel;
        this.totalIncome = totalIncome;
        this.yesterdaySales = yesterdaySales;
        this.selectedDishes = selectedDishes == null
            ? new List<DishType>()
            : new List<DishType>(selectedDishes);
    }

    public bool SelectDish(DishType dishType, int dishLevel)
    {
        if (dishLevel <= 0 || selectedDishes.Contains(dishType))
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
