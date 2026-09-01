using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MarketManager : MonoBehaviour
{
    public const int MaxMarketLevel = 4;

    #region Fields & Properties

    [SerializeField] private MarketData marketData = new();
    [SerializeField] private LevelData levelData = new();
    [SerializeField] private FestivalCalendar festivalCalendar = new();
    [FormerlySerializedAs("levelMissionChecker")]
    [SerializeField] private LevelMissionProgress levelMissionProgress = new();

    private Action onMarketDataChanged;

    public MarketData MarketData => marketData;
    public LevelData LevelData => levelData;
    public FestivalCalendar FestivalCalendar => festivalCalendar;
    public LevelMissionProgress LevelMissionProgress => levelMissionProgress;
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

            if (!LevelMissionGroupDB.TryGetData(nextLevel, out LevelMissionGroupSO nextMissionGroup)
                || nextMissionGroup == null
                || nextMissionGroup.Missions == null
                || nextMissionGroup.Missions.Count == 0)
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
        festivalCalendar ??= new FestivalCalendar();
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
                marketData.CurrentBusinessDay++;
                marketData.CurrentPhase = MarketPhase.Morning;
                break;
        }
    }

    public void LevelRefresh()
    {
        levelData = LevelDataDB.GetData(marketData.CurrentLevel) ?? new LevelData();
        levelMissionProgress.SetMissionGroup(
            LevelMissionGroupDB.GetData(marketData.CurrentLevel));

        GameManager.Instance?.Upgrade?.RefreshRuntimeData();
    }

    public bool CanStartTasteFestival(TasteType taste)
    {
        return festivalCalendar.CanStartTasteFestival(
            taste,
            marketData.CurrentBusinessDay);
    }

    public bool TryStartTasteFestival(TasteType taste)
    {
        if (!festivalCalendar.TryStartTasteFestival(
                taste,
                marketData.CurrentBusinessDay))
            return false;

        NotifyMarketDataChanged();
        return true;
    }

    public bool CanStartCategoryFestival(CategoryType category)
    {
        return festivalCalendar.CanStartCategoryFestival(
            category,
            marketData.CurrentBusinessDay);
    }

    public bool TryStartCategoryFestival(CategoryType category)
    {
        if (!festivalCalendar.TryStartCategoryFestival(
                category,
                marketData.CurrentBusinessDay))
            return false;

        NotifyMarketDataChanged();
        return true;
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

        if (!LevelMissionGroupDB.TryGetData(
                nextLevel,
                out LevelMissionGroupSO nextMissionGroup))
        {
            return false;
        }

        levelData = nextLevelData;
        levelMissionProgress.SetMissionGroup(nextMissionGroup);
        levelMissionProgress.LoadClaimedMissionCount(0);
        marketData.CurrentLevel = nextLevel;
        GameManager.Instance?.Upgrade?.RefreshRuntimeData();
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
            yesterdaySales = marketData.YesterdaySales,
            claimedMissionCount = levelMissionProgress.ClaimedMissionCount,
            festivalStateVersion = 1,
            latestFestivalTaste = festivalCalendar.LatestTaste,
            tasteFestivalStartBusinessDay = festivalCalendar.TasteStartBusinessDay,
            latestFestivalCategory = festivalCalendar.LatestCategory,
            categoryFestivalStartBusinessDay = festivalCalendar.CategoryStartBusinessDay
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
                Mathf.Max(0, saveData.yesterdaySales),
                saveData.selectedDishes);

        ReplaceMarketData(loadedData);
        LevelRefresh();
        levelMissionProgress.LoadClaimedMissionCount(
            saveData == null ? 0 : saveData.claimedMissionCount);

        if (saveData != null && saveData.festivalStateVersion > 0)
        {
            festivalCalendar.Load(
                saveData.latestFestivalTaste,
                saveData.tasteFestivalStartBusinessDay,
                saveData.latestFestivalCategory,
                saveData.categoryFestivalStartBusinessDay);
        }
        else
        {
            festivalCalendar.Reset();
        }

        NotifyMarketDataChanged();
    }

    public void ResetMarketSaveData()
    {
        ReplaceMarketData(new MarketData());
        LevelRefresh();
        levelMissionProgress.LoadClaimedMissionCount(0);
        festivalCalendar.Reset();
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
