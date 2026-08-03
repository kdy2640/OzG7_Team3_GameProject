using System.Collections.Generic;
using UnityEngine;

public enum DishType
{
    Steak,
    Bread,
    Hamburger,
    Count
}

[CreateAssetMenu(menuName = "Game/DishDataSO")]
public class DishDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private DishType dish;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private List<GroceryAmount> ingredients = new();
    [SerializeField, Min(0)] private float cost;

    public string Id => id;
    public DishType Dish => dish;
    public string DisplayName => displayName;
    public string Description => description;
    public List<GroceryAmount> Ingredients => ingredients;
    public float Cost => cost;
}
