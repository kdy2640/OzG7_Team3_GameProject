using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class MarketData
{
    [SerializeField, Min(0)] private int currentBusinessDay;
    [SerializeField, Min(0)] private int currentLevel;
    [SerializeField, Min(0)] private int currentEXP;
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

    public int CurrentEXP
    {
        get => currentEXP;
        set
        {
            int nextEXP = Mathf.Max(0, value);

            if (currentEXP == nextEXP)
                return;

            currentEXP = nextEXP;
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
        int currentEXP,
        List<DishType> selectedDishes)
    {
        this.currentBusinessDay = currentBusinessDay;
        this.currentLevel = currentLevel;
        this.currentEXP = currentEXP;
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
