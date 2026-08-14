using System;
using UnityEngine;

public class FacilityController : MonoBehaviour
{
    [Header("Facility")]
    [SerializeField]
    private FacilityType facilityType = FacilityType.Count;

    [Header("View")]
    [SerializeField] private FacilityModelView modelView;
    [SerializeField] private FacilityWorldUI worldUI;

    private bool isUpgradeManagerSubscribed;

    private UpgradeManager SubscribedUpgradeManager => GameManager.Instance?.Upgrade;

    private FacilityUpgradeDataSO UpgradeData => UpgradeDataDB.GetData(facilityType);

    public FacilityType FacilityType => facilityType;

    public string FacilityName
    {
        get
        {
            FacilityDataSO data = FacilityDataDB.GetData(facilityType);

            return data != null
                ? data.DisplayName : facilityType.ToString();
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

    #region Unity Lifecycle

    private void OnEnable()
    {
        TrySubscribeUpgradeManager();
        RefreshFromUpgradeManager();
    }

    private void Start()
    {
        // UI 프리팹이 런타임에 생성되는 구조에서는
        // OnEnable 시점보다 UpgradeManager가 늦게 준비될 수 있으므로
        // 한 번 더 확인합니다.
        TrySubscribeUpgradeManager();
        RefreshFromUpgradeManager();
    }

    private void OnDisable()
    {
        UnsubscribeUpgradeManager();
    }

    #endregion

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
        UpgradeManager upgradeManager =
            SubscribedUpgradeManager;

        if (upgradeManager == null)
            return UpgradeAvailability.InvalidData;

        if (UpgradeData == null)
            return UpgradeAvailability.InvalidData;

        return upgradeManager.GetUpgradeAvailability(
            UpgradeData);
    }

    private bool TryUpgradeInternal()
    {
        UpgradeManager upgradeManager =
            SubscribedUpgradeManager;

        if (upgradeManager == null)
        {
            Debug.LogError(
                $"[FacilityController] UpgradeManager를 찾을 수 없습니다. " +
                $"Facility : {facilityType}",
                this);

            return false;
        }

        FacilityUpgradeDataSO data =
            UpgradeData;

        if (data == null)
        {
            Debug.LogError(
                $"[FacilityController] FacilityUpgradeDataSO를 찾을 수 없습니다. " +
                $"Facility : {facilityType}",
                this);

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
            $"MaxLevel={data.MaxLevel}",
            this);

        if (availability != UpgradeAvailability.Available)
            return false;

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

    #region Upgrade Event

    private void TrySubscribeUpgradeManager()
    {
        if (isUpgradeManagerSubscribed)
            return;

        UpgradeManager upgradeManager =
            GameManager.Instance?.Upgrade;

        if (upgradeManager == null)
        {
            Debug.LogWarning(
                $"[FacilityController] UpgradeManager가 아직 준비되지 않았습니다. " +
                $"Facility={facilityType}",
                this);

            return;
        }

        upgradeManager.SubscribeUpgradeChanged(
            RefreshFromUpgradeManager);

        isUpgradeManagerSubscribed = true;

        Debug.Log(
            $"[FacilityController] UpgradeChanged 구독 완료: {facilityType}",
            this);
    }

    private void UnsubscribeUpgradeManager()
    {
        if (!isUpgradeManagerSubscribed)
            return;

        UpgradeManager upgradeManager =
            GameManager.Instance?.Upgrade;

        if (upgradeManager != null)
        {
            upgradeManager.UnsubscribeUpgradeChanged(
                RefreshFromUpgradeManager);
        }

        isUpgradeManagerSubscribed = false;
    }

    #endregion

    #region Refresh

    private void RefreshFromUpgradeManager()
    {
        int level = CurrentLevel;

        Debug.Log(
            $"[FacilityController] UI 갱신: " +
            $"Facility={facilityType}, " +
            $"Level={level}, " +
            $"ModelView={(modelView != null)}, " +
            $"WorldUI={(worldUI != null)}",
            this);

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