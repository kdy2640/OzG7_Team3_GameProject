using System.Collections.Generic;
using UnityEngine;

public enum HarvestType
{
    Rice,
    Carrot,
    Chicken,
    Wheat,
    Onion,
    Cow,
    Potato,
    Cabbage,
    Sheep,
    Corn,
    Tomato,
    Grape,
    Pig,
    Count
}

[CreateAssetMenu(menuName = "Game/HarvestDataSO")]
public sealed class HarvestDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private HarvestType harvestType = HarvestType.Count;
    [SerializeField] private List<GroceryAmount> rewards = new();
    [SerializeField] private bool isMove;
    [SerializeField, Min(0f)] private float speed;
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private GameObject solidPrefab;
    [SerializeField] private GameObject itemPrefab;

    public string Id => id;
    public HarvestType HarvestType => harvestType;
    public List<GroceryAmount> Rewards => rewards;
    public bool IsMove => isMove;
    public float Speed => speed;
    public RuntimeAnimatorController AnimatorController => animatorController;
    public GameObject SolidPrefab => solidPrefab;
    public GameObject ItemPrefab => itemPrefab;
}
