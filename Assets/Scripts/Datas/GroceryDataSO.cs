using UnityEngine;

public enum GroceryType
{
    Rice,
    Carrot,
    Chicken,
    Wheat,
    Onion,
    Beef,
    Potato,
    Cabbage,
    Lamb,
    Count
}

[CreateAssetMenu(menuName = "Game/GroceryDataSO")]
public sealed class GroceryDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private GroceryType grocery = GroceryType.Count;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField, Min(0)] private float cost;

    public string Id => id;
    public GroceryType Grocery => grocery;
    public string DisplayName => displayName;
    public string Description => description;
    public float Cost => cost;
}
