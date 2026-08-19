using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ServiceResultData
{
    public int tipResult;
    public int customerReceived;
    public int customerMax;

    public List<MenuSalesResultData> menuSales = new();
}

[Serializable]
public sealed class MenuSalesResultData
{
    public DishType dishType;
    public string menuName;
    public Sprite menuIcon;
    public int salesAmount;
}