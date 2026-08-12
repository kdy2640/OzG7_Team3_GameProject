
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
    Decor_3,
    Decor_4,
    Decor_5,
    Decor_6,
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

    [SerializeField] private List<GameObject> solidPrefabs = new();

    public List<GameObject> SolidPrefabs => solidPrefabs;

    public string Id => id;
    public string DisplayName => displayName;
    public FacilityType FacilityType => facilityType;
    public FacilityCategory FacilityCategory => facilityCategory;
    public int Cost => cost;
    public float UpgradeMultiplier => upgradeMultiplier;

    // 등록된 모델 수 = 표현 가능한 최대 레벨
    public int MaxVisualLevel => 
        solidPrefabs == null ? 0 : Mathf.Max(0, solidPrefabs.Count - 1);
    //visualLevel : 0 = 미구매/잠금 상태, 1 = Lv.1 모델, 2 = Lv.2 모델 ...
    public GameObject GetSolidPrefabForLevel(int visualLevel)
    {
        if (solidPrefabs == null || visualLevel<0 || solidPrefabs.Count <= visualLevel)
            return null;
        //Lv.0은 [0], Lv.1은 [1], Lv.2는 [2]...
        return solidPrefabs[visualLevel];
    }

}
