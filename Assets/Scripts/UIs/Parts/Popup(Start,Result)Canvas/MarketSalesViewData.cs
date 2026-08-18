using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public sealed class MarketSalesViewData
{
    public int todaySales;
    public int yesterdaySales;
    public int servedCustomerCount;
    public int tipSales;

    public List<MenuSalesViewData> menuSales = new();

    [Serializable]
    public sealed class MenuSalesViewData
    {
        public Sprite menuIcon;
        public int salesAmount;
    }
}
