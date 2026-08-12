using System;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    #region Fields & Properties

    [SerializeField] private MarketData marketData = new();
    [SerializeField] private LevelData levelData = new();
    [SerializeField] private LevelMissionChecker levelMissionChecker = new();

    private Action onMarketDataChanged;

    public MarketData MarketData => marketData;
    public LevelData LevelData => levelData;
    public LevelMissionChecker LevelMissionChecker => levelMissionChecker;
    public LevelMissionGroupSO LevelMissionGroup => levelMissionChecker.MissionGroup;
    public int CurrentBusinessDay => marketData.CurrentBusinessDay;
    public TasteType TodayTaste => (TasteType)(CurrentBusinessDay % (int)TasteType.Count);

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        marketData ??= new MarketData();
        levelMissionChecker ??= new LevelMissionChecker();
        SubscribeMarketData();
    }

    private void Start()
    {
        LevelRefresh();
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

    public void MoveToNextPhase()
    {
        switch (marketData.CurrentPhase)
        {
            case MarketPhase.Morning:
                marketData.CurrentPhase = MarketPhase.Afternoon;
                break;
            case MarketPhase.Afternoon:
                marketData.CurrentPhase = MarketPhase.Night;
                break;
            case MarketPhase.Night:
                marketData.CurrentBusinessDay++;
                marketData.CurrentPhase = MarketPhase.Morning;
                break;
        }
    }

    public void LevelRefresh()
    {
        levelData = LevelDataDB.GetData(marketData.CurrentLevel) ?? new LevelData();
        levelMissionChecker.SetMissionGroup(LevelMissionGroupDB.GetData(marketData.CurrentLevel));
    }

    #endregion

    #region Save Data

    public MarketSaveData CreateMarketSaveData()
    {
        MarketSaveData saveData = new()
        {
            currentBusinessDay = marketData.CurrentBusinessDay,
            currentPhase = marketData.CurrentPhase,
            currentLevel = marketData.CurrentLevel,
            totalIncome = marketData.TotalIncome
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
                saveData.currentPhase,
                Mathf.Max(0, saveData.currentLevel),
                Mathf.Max(0, saveData.totalIncome),
                saveData.selectedDishes);

        ReplaceMarketData(loadedData);
        LevelRefresh();
        NotifyMarketDataChanged();
    }

    public void ResetMarketSaveData()
    {
        ReplaceMarketData(new MarketData());
        LevelRefresh();
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
