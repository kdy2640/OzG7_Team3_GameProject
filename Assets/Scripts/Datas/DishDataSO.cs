using System.Collections.Generic;
using UnityEngine;

public enum DishType
{
    None = -1,
    MeatOnigiri,
    VegetableGimbap,
    Steak,
    Meatloaf,
    OnionBagel,
    LambSteak,
    PotatoNoodles,
    LambSoup,
    Count
}

public enum TasteType
{
    Salty,
    Sweet,
    Spicy,
    Count
}

[CreateAssetMenu(menuName = "Game/DishDataSO")]
public class DishDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private DishType dish;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private List<TasteType> tastes = new();
    [SerializeField] private List<GroceryAmount> ingredients = new();
    [SerializeField, Min(0)] private float cost;
    [SerializeField] private Sprite icon;

    public string Id => id;
    public DishType Dish => dish;
    public string DisplayName => displayName;
    public string Description => description;
    public List<TasteType> Tastes => tastes;
    public List<GroceryAmount> Ingredients => ingredients;
    public float Cost => cost;
    public Sprite Icon => icon;
}
