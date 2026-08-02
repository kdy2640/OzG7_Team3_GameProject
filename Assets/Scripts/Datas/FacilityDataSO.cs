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
    public string id;
    public string displayName;
    public FacilityType facilityType = FacilityType.Count;
    public FacilityCategory facilityCategory = FacilityCategory.Count;
    [Min(0)] public int cost;
    [Min(0f)] public float upgradeMultiplier = 1f;
    [Min(1)] public int maxLevel = 1;
}
