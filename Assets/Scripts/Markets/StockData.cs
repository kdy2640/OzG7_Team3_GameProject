using System;
using System.Collections.Generic;
using UnityEngine;

public enum GroceryType
{
    Wheat,
    Beaf,
    Count
}

public enum DishType
{
    Steak,
    Bread,
    Hamburger,
    Count
}

[Serializable]
public class GroceryAmount
{
    public GroceryType grocery;
    [Min(0)] public int amount;

    public GroceryAmount()
    {
    }

    // 코드에서 식재료 수량을 만들 때 사용.
    public GroceryAmount(GroceryType grocery, int amount)
    {
        this.grocery = grocery;
        this.amount = amount;
    }
}

[Serializable]
public class DishAmount
{
    public DishType dish;
    [Min(0)] public int amount;

    public DishAmount()
    {
    }

    // 코드에서 요리 수량을 만들 때 사용.
    public DishAmount(DishType dish, int amount)
    {
        this.dish = dish;
        this.amount = amount;
    }
}

public interface IReadableStockData
{
    // 현재 재화를 표시할 때 사용.
    float Currency { get; }

    // 현재 식재료 재고를 표시할 때 사용.
    IReadOnlyList<GroceryAmount> Groceries { get; }

    // 현재 요리 재고를 표시할 때 사용.
    IReadOnlyList<DishAmount> Dishes { get; }
}

[Serializable]
public class StockData : IReadableStockData
{
    [Min(0)] public float currency;
    public List<GroceryAmount> groceries = new();
    public List<DishAmount> dishes = new();

    public float Currency => currency;
    public IReadOnlyList<GroceryAmount> Groceries => groceries;
    public IReadOnlyList<DishAmount> Dishes => dishes;
}
