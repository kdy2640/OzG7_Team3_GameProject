using UnityEngine;

public sealed class RestaurantModelViewer : MonoBehaviour
{
    [Header("Environment")]
    [SerializeField] private Transform environmentRoot;
    [SerializeField] private GameObject[] modelPrefabsByMarketLevel =
        new GameObject[MarketManager.MaxMarketLevel + 1];
    [SerializeField] private bool collidersEnabled = true;

    private MarketManager marketManager;
    private GameObject currentModelInstance;
    private int shownLevel = -1;

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

    private void Start()
    {
        ApplyColliderState();
    }

    public void Refresh()
    {
        ShowLevel(marketManager.MarketData.CurrentLevel);
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
        ApplyColliderState();
    }

    private void ApplyColliderState()
    {
        if (collidersEnabled)
            return;

        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider targetCollider in colliders)
            targetCollider.enabled = false;
    }

    private void ClearCurrentModel()
    {
        if (currentModelInstance != null)
            Destroy(currentModelInstance);

        currentModelInstance = null;
        shownLevel = -1;
    }
}
