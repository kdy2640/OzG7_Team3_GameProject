using UnityEngine;

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
    [SerializeField, Min(1)] private int maxLevel = 1;

    public string Id => id;
    public string DisplayName => displayName;
    public FacilityType FacilityType => facilityType;
    public FacilityCategory FacilityCategory => facilityCategory;
    public int Cost => cost;
    public float UpgradeMultiplier => upgradeMultiplier;
    public int MaxLevel => maxLevel;
}
