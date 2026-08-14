using System.Collections.Generic;
using UnityEngine;

public enum DishType
{
    None = -1,
    Croquette,
    Sandwich,
    Taco,
    Hamburger,
    Pho,
    LambSoup,
    LambPie,
    PotatoNoodle,
    FriedChicken,
    Meatloaf,
    MeatCurry,
    OnionBagel,
    ChickenPorridge,
    VegetableGimbap,
    CarrotSalad,
    Count
}

public enum TasteType
{
    Salty,
    Clean,
    SpicyAndSour,
    Count
}
public enum CategoryType
{
    WesternDine,
    AsianFood,
    StreetSnack,
    Count
}

[CreateAssetMenu(menuName = "Game/DishDataSO")]
public class DishDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private DishType dish;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private GroceryAmountWrapper ingredients;
    [SerializeField, Min(0)] private int cost;
    [SerializeField] private int tier;
    [SerializeField] private TasteType tastes;
    [SerializeField] private CategoryType category;
    [SerializeField] private Sprite icon;

    public string Id => id;
    public DishType Dish => dish;
    public string DisplayName => displayName;
    public string Description => description;
    public List<GroceryAmount> Ingredients => ingredients.value;
    public int Cost => cost;
    public int Tier => tier;
    public TasteType Tastes => tastes;
    public CategoryType Category => category;
    public Sprite Icon => icon;
}

[System.Serializable] // 0814 장은수
public class GroceryAmountWrapper
{
    public List<GroceryAmount> value;
}