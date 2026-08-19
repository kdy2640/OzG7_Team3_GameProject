using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_MareketVisualPanel : MonoBehaviour
{
    [SerializeField] private Image[] levelSlots;
    [SerializeField] private GameObject salesProgressPanel;
    [SerializeField] private Slider salesSlider;
    [SerializeField] private TMP_Text salesAmountText;
    [SerializeField] private GameObject missionSlotContainer;
    [SerializeField] private Image[] questSlots;
    [SerializeField] private TMP_Text missionTitleText;
    [SerializeField] private TMP_Text missionDescriptionText;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;

    private MarketManager marketManager;
    private UpgradeManager upgradeManager;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        marketManager = GameManager.Instance.Market;
        upgradeManager = GameManager.Instance.Upgrade;

        marketManager?.SubscribeMarketDataChanged(Refresh);
        upgradeManager?.SubscribeUpgradeChanged(Refresh);
        Refresh();
    }

    private void OnDisable()
    {
        marketManager?.UnsubscribeMarketDataChanged(Refresh);
        upgradeManager?.UnsubscribeUpgradeChanged(Refresh);

        marketManager = null;
        upgradeManager = null;
    }

    public void Refresh()
    {
        if (GameManager.Instance == null || GameManager.Instance.Market == null)
            return;

        MarketManager market = GameManager.Instance.Market;
        MarketData marketData = market.MarketData;
        LevelData levelData = market.LevelData;

        int levelSlotCount = levelSlots?.Length ?? 0;
        int activeLevelCount = Mathf.Clamp(marketData.CurrentLevel, 0, levelSlotCount);

        for (int i = 0; i < levelSlotCount; i++)
            SetSlotColor(levelSlots[i], i < activeLevelCount);

        bool isMaxLevel = marketData.CurrentLevel >= MarketManager.MaxMarketLevel;

        if (salesProgressPanel != null)
            salesProgressPanel.SetActive(!isMaxLevel);

        if (missionSlotContainer != null)
            missionSlotContainer.SetActive(!isMaxLevel);

        if (isMaxLevel)
        {
            for (int i = 0; i < (questSlots?.Length ?? 0); i++)
            {
                if (questSlots[i] != null)
                    questSlots[i].gameObject.SetActive(false);
            }

            if (missionTitleText != null)
                missionTitleText.text = "Max Level";

            if (missionDescriptionText != null)
                missionDescriptionText.text = string.Empty;

            return;
        }

        int incomeGoal = Mathf.Max(1, levelData.IncomeGoal);
        int totalIncome = Mathf.Clamp(marketData.TotalIncome, 0, incomeGoal);

        if (salesSlider != null)
        {
            salesSlider.minValue = 0f;
            salesSlider.maxValue = incomeGoal;
            salesSlider.value = totalIncome;
        }

        if (salesAmountText != null)
            salesAmountText.text = $"{totalIncome:N0} / {incomeGoal:N0}";

        LevelMissionGroupSO missionGroup = market.LevelMissionGroup;
        int questSlotCount = questSlots?.Length ?? 0;
        int missionCount = Mathf.Clamp(
            missionGroup == null || missionGroup.Missions == null
                ? 0
                : missionGroup.Missions.Count,
            0,
            questSlotCount);
        int currentStage = Mathf.Clamp(
            market.LevelMissionProgress.CurrentStage,
            0,
            missionCount);

        for (int i = 0; i < questSlotCount; i++)
        {
            Image questSlot = questSlots[i];

            if (questSlot == null)
                continue;

            questSlot.gameObject.SetActive(i < missionCount);
            SetSlotColor(questSlot, i < currentStage);
        }

        LevelMissionInfo currentMission = market.LevelMissionProgress.CurrentMission;

        if (currentMission != null)
        {
            if (missionTitleText != null)
                missionTitleText.text = currentMission.Title;

            if (missionDescriptionText != null)
            {
                string progress = currentMission.Condition?.ToString() ?? string.Empty;
                missionDescriptionText.text = string.IsNullOrEmpty(progress)
                    ? currentMission.Description
                    : $"{currentMission.Description}\n{progress}";
            }

            return;
        }

        bool areAllMissionsCompleted = market.LevelMissionProgress.AreAllMissionsClaimed;
        bool incomeGoalReached = levelData.IncomeGoal > 0
            && marketData.TotalIncome >= levelData.IncomeGoal;
        bool canPromote = market.CanPromote;

        if (missionTitleText != null)
        {
            if (!areAllMissionsCompleted)
                missionTitleText.text = "Promotion Mission Data Missing";
            else if (canPromote)
                missionTitleText.text = "Promotion Ready";
            else if (!incomeGoalReached)
                missionTitleText.text = "All Promotion Missions Complete";
            else
                missionTitleText.text = "Promotion Data Missing";
        }

        if (missionDescriptionText != null)
        {
            missionDescriptionText.text = areAllMissionsCompleted && !incomeGoalReached
                ? "Reach the sales goal."
                : string.Empty;
        }
    }

    public void ClaimCurrentMissionReward()
    {
        marketManager?.TryClaimCurrentMissionReward();
    }

    private void SetSlotColor(Image slot, bool isActive)
    {
        if (slot != null)
            slot.color = isActive ? activeColor : inactiveColor;
    }
}
