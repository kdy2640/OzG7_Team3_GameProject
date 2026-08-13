using System;
using UnityEngine;

public class FacilityController : MonoBehaviour
{
    [Header("Facility")]
    [SerializeField] private FacilityType facilityType = FacilityType.Count;

    [Header("View")]
    [SerializeField] private FacilityModelView modelView;
    [SerializeField] private FacilityWorldUI worldUI;

    private UpgradeManager SubscribedUpgradeManager => GameManager.Instance?.Upgrade;

    private FacilityUpgradeDataSO UpgradeData => UpgradeDataDB.GetData(facilityType);

    public FacilityType FacilityType => facilityType;

    public string FacilityName
    {
        get
        {
            FacilityDataSO data = FacilityDataDB.GetData(facilityType);

            return data != null ? data.DisplayName : facilityType.ToString();
        }
    }

    public int CurrentLevel
    {
        get
        {
            UpgradeManager upgradeManager = SubscribedUpgradeManager;

            if (upgradeManager == null) return 0;

            return upgradeManager.RuntimeLevel.Get(facilityType);
        }
    }

    public bool IsPurchased => CurrentLevel > 0;

    public int MaxLevel => UpgradeData != null ? UpgradeData.MaxLevel : 0;

    public event Action<FacilityController> StateChanged;

    private void OnEnable()
    {
        SubscribeUpgradeManager();
        RefreshFromUpgradeManager();
    }

    private void OnDisable()
    {
        UnsubscribeUpgradeManager();
    }

    #region Upgrade

    public bool TryPurchase()
    {
        if (IsPurchased) return false;

        return TryUpgradeInternal();
    }

    public bool TryUpgrade()
    {
        if (!IsPurchased) return false;

        return TryUpgradeInternal();
    }

    public bool CanUpgrade()
    {
        return GetUpgradeAvailability() == UpgradeAvailability.Available;
    }

    public UpgradeAvailability GetUpgradeAvailability()
    {
        UpgradeManager upgradeManager = SubscribedUpgradeManager;

        if (upgradeManager == null)
            return UpgradeAvailability.InvalidData;

        if (UpgradeData == null)
            return UpgradeAvailability.InvalidData;

        return upgradeManager.GetUpgradeAvailability(UpgradeData);
    }

    private bool TryUpgradeInternal()
    {
        UpgradeManager upgradeManager = SubscribedUpgradeManager;

        if (upgradeManager == null)
        {
            Debug.LogError(
                $"[FacilityController] UpgradeManager를 찾을 수 없습니다. " +
                $"Facility : {facilityType}");

            return false;
        }

        FacilityUpgradeDataSO data = UpgradeData;

        if (data == null)
        {
            Debug.LogError(
                $"[FacilityController] FacilityUpgradeDataSO를 찾을 수 없습니다. " +
                $"Facility : {facilityType}");

            return false;
        }

        UpgradeAvailability availability =
            upgradeManager.GetUpgradeAvailability(data);

        Debug.Log(
            $"[FacilityController] " +
            $"Facility={facilityType}, " +
            $"UpgradeData={data.Id}, " +
            $"Availability={availability}, " +
            $"CurrentLevel={CurrentLevel}, " +
            $"MaxLevel={data.MaxLevel}");

        if (availability != UpgradeAvailability.Available) return false;

        return upgradeManager.TryUpgrade(data);
    }

    #endregion

    #region Effect

    public string GetCurrentEffect()
    {
        if (!IsPurchased)
            return "Not Purchased";

        return $"Lv.{CurrentLevel}";
    }

    public string GetNextEffect()
    {
        if (!IsPurchased)
            return "Purchase to unlock";

        if (!CanUpgrade())
            return "Max Level";

        return $"Lv.{CurrentLevel + 1}";
    }

    #endregion

    #region Refresh

    private void SubscribeUpgradeManager()
    {
        UpgradeManager upgradeManager = SubscribedUpgradeManager;

        if (upgradeManager == null)
            return;

        upgradeManager.SubscribeUpgradeChanged(
            RefreshFromUpgradeManager);
    }

    private void UnsubscribeUpgradeManager()
    {
        UpgradeManager upgradeManager = SubscribedUpgradeManager;

        if (upgradeManager == null)
            return;

        upgradeManager.UnsubscribeUpgradeChanged(
            RefreshFromUpgradeManager);
    }

    private void RefreshFromUpgradeManager()
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

    #endregion
}