using System;
using System.Collections.Generic;

[Serializable]
public sealed class SalesResultData
{
    [Serializable]
    public sealed class MenuSalesData
    {
        public DishType dishType;
        public int salesAmount;
    }

    public int todaySales;
    public int yesterdaySales;
     
    public int customerReceived;
    public int customerMax;

    public int tipSales;

    public List<MenuSalesData> menuSales = new();
}
