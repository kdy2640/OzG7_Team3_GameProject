using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MarketManager : MonoBehaviour
{
    public const int MaxMarketLevel = 4;

    #region Fields & Properties

    [SerializeField] private MarketData marketData = new();
    [SerializeField] private LevelData levelData = new();
    [FormerlySerializedAs("levelMissionChecker")]
    [SerializeField] private LevelMissionProgress levelMissionProgress = new();

    private Action onMarketDataChanged;

    public MarketData MarketData => marketData;
    public LevelData LevelData => levelData;
    public LevelMissionProgress LevelMissionProgress => levelMissionProgress;
    public LevelMissionGroupSO LevelMissionGroup => levelMissionProgress.MissionGroup;
    public int CurrentBusinessDay => marketData.CurrentBusinessDay;
    public TasteType TodayTaste => (TasteType)(CurrentBusinessDay % (int)TasteType.Count);
    public bool CanPromote
    {
        get
        {
            if (marketData.CurrentLevel >= MaxMarketLevel
                || levelData.IncomeGoal <= 0
                || marketData.TotalIncome < levelData.IncomeGoal
                || !levelMissionProgress.AreAllMissionsClaimed)
            {
                return false;
            }

            int nextLevel = marketData.CurrentLevel + 1;

            if (!LevelDataDB.TryGetData(nextLevel, out _))
                return false;

            if (nextLevel < MaxMarketLevel
                && (!LevelMissionGroupDB.TryGetData(nextLevel, out LevelMissionGroupSO nextMissionGroup)
                    || nextMissionGroup == null
                    || nextMissionGroup.Missions == null
                    || nextMissionGroup.Missions.Count == 0))
            {
                return false;
            }

            return true;
        }
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        marketData ??= new MarketData();
        levelMissionProgress ??= new LevelMissionProgress();
        SubscribeMarketData();
    }

    private void Start()
    {
        LevelRefresh();
        NotifyMarketDataChanged();
    }

    private void OnDestroy()
    {
        if (marketData != null)
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;
    }

    #endregion

    #region Runtime Data

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
                TryPromote();
                marketData.CurrentBusinessDay++;
                marketData.CurrentPhase = MarketPhase.Morning;
                break;
        }
    }

    public void LevelRefresh()
    {
        levelData = LevelDataDB.GetData(marketData.CurrentLevel) ?? new LevelData();
        levelMissionProgress.SetMissionGroup(
            marketData.CurrentLevel < MaxMarketLevel
                ? LevelMissionGroupDB.GetData(marketData.CurrentLevel)
                : null);
    }

    public bool TryClaimCurrentMissionReward()
    {
        if (!levelMissionProgress.TryClaimCurrentReward())
            return false;

        NotifyMarketDataChanged();
        return true;
    }

    public bool TryPromote()
    {
        if (!CanPromote)
            return false;

        int nextLevel = marketData.CurrentLevel + 1;

        if (!LevelDataDB.TryGetData(nextLevel, out LevelData nextLevelData))
            return false;

        LevelMissionGroupSO nextMissionGroup = null;

        if (nextLevel < MaxMarketLevel
            && !LevelMissionGroupDB.TryGetData(nextLevel, out nextMissionGroup))
        {
            return false;
        }

        levelData = nextLevelData;
        levelMissionProgress.SetMissionGroup(nextMissionGroup);
        levelMissionProgress.LoadClaimedMissionCount(0);
        marketData.CurrentLevel = nextLevel;
        return true;
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
            totalIncome = marketData.TotalIncome,
            claimedMissionCount = levelMissionProgress.ClaimedMissionCount
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
                Mathf.Clamp(saveData.currentLevel, 0, MaxMarketLevel),
                Mathf.Max(0, saveData.totalIncome),
                saveData.selectedDishes);

        ReplaceMarketData(loadedData);
        LevelRefresh();
        levelMissionProgress.LoadClaimedMissionCount(
            saveData == null ? 0 : saveData.claimedMissionCount);
        NotifyMarketDataChanged();
    }

    public void ResetMarketSaveData()
    {
        ReplaceMarketData(new MarketData());
        LevelRefresh();
        levelMissionProgress.LoadClaimedMissionCount(0);
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
