using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DishDataSO")]
public class DishDataSO : ScriptableObject
{
    public string id;
    public DishType dish;
    public string displayName;
    [TextArea] public string description;
    public List<GroceryAmount> ingredients = new();
    [Min(0)] public float cost;
}
