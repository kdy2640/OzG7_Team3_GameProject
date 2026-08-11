using System;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    #region Fields & Properties

    [SerializeField] private MarketData marketData = new();
    [SerializeField] private LevelData levelData = new();

    private Action onMarketDataChanged;

    public MarketData MarketData => marketData;
    public LevelData LevelData => levelData;
    public int CurrentBusinessDay => marketData.CurrentBusinessDay;
    public TasteType TodayTaste => (TasteType)(CurrentBusinessDay % (int)TasteType.Count);

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        marketData ??= new MarketData();
        SubscribeMarketData();
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        if (marketData != null)
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;
    }

    #endregion

    #region Runtime Data

    public void CompleteCurrentBusinessDay()
    {
        marketData.CurrentBusinessDay++;
    }

    public void Refresh()
    {
        levelData = LevelDataDB.GetData(marketData.CurrentLevel) ?? new LevelData();
    }

    #endregion

    #region Save Data

    public MarketSaveData CreateMarketSaveData()
    {
        MarketSaveData saveData = new()
        {
            currentBusinessDay = marketData.CurrentBusinessDay,
            currentLevel = marketData.CurrentLevel,
            currentEXP = marketData.CurrentEXP
        };

        saveData.selectedDishes.AddRange(marketData.SelectedDishes);

        return saveData;
    }

    public void LoadMarketSaveData(MarketSaveData saveData)
    {
        MarketData loadedData = saveData == null
            ? new MarketData()
            : new MarketData(
                Mathf.Max(0, saveData.currentBusinessDay),
                Mathf.Max(0, saveData.currentLevel),
                Mathf.Max(0, saveData.currentEXP),
                saveData.selectedDishes);

        ReplaceMarketData(loadedData);
        Refresh();
        NotifyMarketDataChanged();
    }

    public void ResetMarketSaveData()
    {
        ReplaceMarketData(new MarketData());
        Refresh();
        NotifyMarketDataChanged();
    }

    #endregion

    #region Events

    public void SubscribeMarketDataChanged(Action callback)
    {
        onMarketDataChanged += callback;
    }

    public void UnsubscribeMarketDataChanged(Action callback)
    {
        onMarketDataChanged -= callback;
    }

    private void ReplaceMarketData(MarketData newMarketData)
    {
        if (marketData != null)
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;

        marketData = newMarketData ?? new MarketData();
        SubscribeMarketData();
    }

    private void SubscribeMarketData()
    {
        marketData.OnMarketDataChanged -= HandleMarketDataChanged;
        marketData.OnMarketDataChanged += HandleMarketDataChanged;
    }

    private void HandleMarketDataChanged()
    {
        NotifyMarketDataChanged();
    }

    private void NotifyMarketDataChanged()
    {
        onMarketDataChanged?.Invoke();
    }

    #endregion
}
