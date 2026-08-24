using UnityEngine;

public sealed class RestaurantModelViewer : MonoBehaviour
{
    [Header("Environment")]
    [SerializeField] private Transform environmentRoot;
    [SerializeField] private GameObject[] modelPrefabsByMarketLevel =
        new GameObject[MarketManager.MaxMarketLevel + 1];

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
    }

    private void ClearCurrentModel()
    {
        if (currentModelInstance != null)
            Destroy(currentModelInstance);

        currentModelInstance = null;
        shownLevel = -1;
    }
}
