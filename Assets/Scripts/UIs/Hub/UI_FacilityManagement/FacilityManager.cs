using System.Collections.Generic;
using UnityEngine;

public class FacilityManager : MonoBehaviour
{
    public static FacilityManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private UpgradeManager upgradeManager;

    private readonly Dictionary<FacilityType, FacilityController> facilities = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // FacilityManager와 UpgradeManager가 같은 GameManager 오브젝트에 있다면 자동 연결
        if (upgradeManager == null) upgradeManager = GetComponent<UpgradeManager>();

        if (upgradeManager == null)
        {
            Debug.LogError
                ("FacilityManager: UpgradeManager를 Inspector에 연결하거나 같은 오브젝트에 붙이세요.");
            return;
        }

        upgradeManager.SubscribeUpgradeChanged(OnUpgradeChanged);
    }

    private void OnDestroy()
    {
        if (upgradeManager != null)
            upgradeManager.UnsubscribeUpgradeChanged(OnUpgradeChanged);

        if (Instance == this) Instance = null;
    }

    public void Register(FacilityController facility)
    {
        if (facility == null || facility.FacilityType == FacilityType.Count)
            return;

        facilities[facility.FacilityType] = facility;
        facility.RefreshFromManager();
    }

    public void Unregister(FacilityController facility)
    {
        if (facility == null) return;

        if (facilities.TryGetValue(facility.FacilityType, out FacilityController registered)
            && registered == facility)
        {
            facilities.Remove(facility.FacilityType);
        }
    }

    public int GetLevel(FacilityType facilityType)
    {
        return upgradeManager == null ? 0 : upgradeManager.GetLevel(facilityType);
    }

    public int GetMaxLevel(FacilityType facilityType)
    {
        FacilityUpgradeDataSO data = UpgradeDataDB.GetData(facilityType);
        return data == null ? 0 : data.MaxLevel;
    }

    public bool IsPurchased(FacilityType facilityType)
    {
        return GetLevel(facilityType) > 0;
    }

    public bool CanUpgrade(FacilityType facilityType)
    {
        int level = GetLevel(facilityType);
        return level > 0 && level < GetMaxLevel(facilityType);
    }

    // Lv.0(구매 전) → Lv.1(구매 완료)
    public bool TryPurchase(FacilityType facilityType)
    {
        if (IsPurchased(facilityType)) return false;

        return TryUpgradeInternal(facilityType);
    }

    // Lv.1 이상 → 다음 레벨
    public bool TryUpgrade(FacilityType facilityType)
    {
        if (!CanUpgrade(facilityType)) return false;

        return TryUpgradeInternal(facilityType);
    }

    private bool TryUpgradeInternal(FacilityType facilityType)
    {
        if (upgradeManager == null) return false;


        FacilityUpgradeDataSO data = UpgradeDataDB.GetData(facilityType);

        if (data == null)
        {
            Debug.LogWarning
                ($"FacilityManager: {facilityType}의 FacilityUpgradeDataSO가 없습니다.");
            return false;
        }
        // 비용 차감, 레벨 증가, 최대 레벨 판정은 UpgradeManager가 처리
        return upgradeManager.TryUpgrade(data);
    }

    private void OnUpgradeChanged()
    {
        foreach (FacilityController facility in facilities.Values)
        {
            if (facility != null) facility.RefreshFromManager();
        }
    }
}