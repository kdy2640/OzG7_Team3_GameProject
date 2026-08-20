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
    Corn,
    Tomato,
    Grape,
    Count
}

[CreateAssetMenu(menuName = "Game/GroceryDataSO")]
public sealed class GroceryDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private GroceryType grocery = GroceryType.Count;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private int tier;

    public string Id => id;
    public GroceryType Grocery => grocery;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public int Tier => tier;
}
