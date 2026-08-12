using System;
using UnityEngine;

public class FacilityController : MonoBehaviour
{
    [SerializeField] private FacilityType facilityType = FacilityType.Count;
    [SerializeField] private FacilityModelView modelView;
    [SerializeField] private FacilityWorldUI worldUI;

    public FacilityType FacilityType => facilityType;
    public string FacilityName
    {
        get
        {
            FacilityDataSO data = FacilityDataDB.GetData(facilityType);
            return data != null ? data.DisplayName : facilityType.ToString();
        }
    }
    public int CurrentLevel => FacilityManager.Instance == null
        ? 0 : FacilityManager.Instance.GetLevel(facilityType);

    public bool IsPurchased => CurrentLevel > 0;

    public int MaxLevel => FacilityManager.Instance == null
        ? 0 : FacilityManager.Instance.GetMaxLevel(facilityType);

    public event Action<FacilityController> StateChanged;

    private void OnEnable()
    {
        FacilityManager.Instance?.Register(this);
    }

    private void Start()
    {
        // FacilityManager 초기화 순서가 늦는 상황 대비
        FacilityManager.Instance?.Register(this);
    }

    private void OnDisable()
    {
        FacilityManager.Instance?.Unregister(this);
    }

    // 버튼/상호작용 코드가 기존처럼 Controller를 호출해도 됩니다.
    public bool TryPurchase()
    {
        return FacilityManager.Instance != null &&
               FacilityManager.Instance.TryPurchase(facilityType);
    }

    public bool TryUpgrade()
    {
        return FacilityManager.Instance != null &&
               FacilityManager.Instance.TryUpgrade(facilityType);
    }

    public bool CanUpgrade()
    {
        return FacilityManager.Instance != null &&
               FacilityManager.Instance.CanUpgrade(facilityType);
    }

    public string GetCurrentEffect()
    {
        if(!IsPurchased) return "Not Purchased";

        return $"Lv.{CurrentLevel}";
    }
    public string GetNextEffect()
    {
        if(!IsPurchased) return "Purchase to unlock";
        if(!CanUpgrade()) return "Max Level";
        return $"Lv.{CurrentLevel + 1}";
    }

    // FacilityManager가 업그레이드 변경 후 호출합니다.
    public void RefreshFromManager()
    {
        int level = CurrentLevel;

        modelView?.ShowLevel(level);

        worldUI?.Refresh(
            isPurchased: level > 0,
            currentLevel: level,
            canUpgrade: CanUpgrade()
        );

        StateChanged?.Invoke(this);
    }
}