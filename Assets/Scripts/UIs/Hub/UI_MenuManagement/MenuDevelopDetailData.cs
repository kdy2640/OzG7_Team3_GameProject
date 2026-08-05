using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MenuDevelopDetailData
{
    public Sprite MenuIcon;

    public string MenuName;

    public int Price;

    public bool CanRegister;

    public List<IngredientCardData> RequiredIngredients = new();
}