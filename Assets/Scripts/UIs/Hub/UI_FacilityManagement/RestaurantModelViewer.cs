using UnityEngine;

public sealed class RestaurantModelViewer : MonoBehaviour
{
    [Header("Environment")]
    [SerializeField] private Transform environmentRoot;
    [SerializeField] private GameObject nightLights;
    [SerializeField] private GameObject[] modelPrefabsByMarketLevel =
        new GameObject[MarketManager.MaxMarketLevel + 1];

    private MarketManager marketManager;
    private GameObject currentModelInstance;
    private int shownLevel = -1;
    private bool isFacilityUpgradeViewEnabled;

    private void Awake()
    {
        SetFacilityUpgradeView(false);
    }

    private void OnEnable()
    {
        marketManager = GameManager.Instance.Market;
        marketManager.SubscribeMarketDataChanged(Refresh);
        Refresh();
    }

    private void OnDisable()
    {
        marketManager.UnsubscribeMarketDataChanged(Refresh);
    }

    public void Refresh()
    {
        nightLights.SetActive(
            marketManager.MarketData.CurrentPhase == MarketPhase.Night);
        ShowLevel(marketManager.MarketData.CurrentLevel);
    }

    public void SetFacilityUpgradeView(bool isEnabled)
    {
        isFacilityUpgradeViewEnabled = isEnabled;

        FacilityRaycaster raycaster =
            GetComponentInChildren<FacilityRaycaster>(true);
        raycaster.enabled = isEnabled;

        FacilityWorldUI[] worldUIs =
            GetComponentsInChildren<FacilityWorldUI>(true);

        foreach (FacilityWorldUI worldUI in worldUIs)
            worldUI.gameObject.SetActive(isEnabled);

        FacilityModelView[] facilityModelViews =
            GetComponentsInChildren<FacilityModelView>(true);

        foreach (FacilityModelView modelView in facilityModelViews)
            modelView.SetFacilityUpgradeView(isEnabled);

        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider targetCollider in colliders)
            targetCollider.enabled = isEnabled;
    }

    private void ShowLevel(int level)
    {
        if (shownLevel == level && currentModelInstance != null)
            return;

        ClearCurrentModel();

        currentModelInstance = Instantiate(
            modelPrefabsByMarketLevel[level],
            environmentRoot,
            false);

        shownLevel = level;
        SetFacilityUpgradeView(isFacilityUpgradeViewEnabled);
    }

    private void ClearCurrentModel()
    {
        if (currentModelInstance != null)
            Destroy(currentModelInstance);

        currentModelInstance = null;
        shownLevel = -1;
    }
}
