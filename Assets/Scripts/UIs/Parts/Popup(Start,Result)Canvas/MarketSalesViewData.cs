using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class MarketSalesViewData
{
    public int todaySales;
    public int yesterdaySales;

    public int servedCustomerCount;
    public int totalCustomerCount;
    
    public int tipSales;

    public List<MenuSalesViewData> menuSales = new();
}

[Serializable]
public sealed class MenuSalesViewData
{
    public DishType dishType;
    public string menuName;
    public Sprite menuIcon;
    public int salesAmount;
}