
using UnityEngine;
using System.Collections.Generic;

public enum FacilityType
{
    Table_1,
    Table_2,
    Table_3,
    Table_4,
    Table_5,
    Table_6,
    Table_7,
    Decor_1,
    Decor_2,
    Count
}

public enum FacilityCategory
{
    Table,
    Decor,
    Count
}

[CreateAssetMenu(menuName = "Game/FacilityDataSO")]
public sealed class FacilityDataSO : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private FacilityType facilityType = FacilityType.Count;
    [SerializeField] private FacilityCategory facilityCategory = FacilityCategory.Count;
    [SerializeField, Min(0)] private int cost;
    [SerializeField, Min(0f)] private float upgradeMultiplier = 1f;

    [SerializeField] private List<GameObject> solidPrefabs = new List<GameObject>();

    public List<GameObject> SolidPrefabs => solidPrefabs;

    public string Id => id;
    public string DisplayName => displayName;
    public FacilityType FacilityType => facilityType;
    public FacilityCategory FacilityCategory => facilityCategory;
    public int Cost => cost;
    public float UpgradeMultiplier => upgradeMultiplier;
}
