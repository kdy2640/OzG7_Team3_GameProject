using System.Collections.Generic;
using UnityEngine;

public enum HarvestType
{
    Wheat,
    Beaf,
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
    [SerializeField] private GameObject solidPrefab;
    [SerializeField] private GameObject itemPrefab;

    public string Id => id;
    public HarvestType HarvestType => harvestType;
    public List<GroceryAmount> Rewards => rewards;
    public bool IsMove => isMove;
    public float Speed => speed;
    public GameObject SolidPrefab => solidPrefab;
    public GameObject ItemPrefab => itemPrefab;
}
