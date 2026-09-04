using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_HarvestStageDetailPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private TMP_Text stageDescriptionText;
    [SerializeField] private Image stageIcon;
    [SerializeField] private UI_GroceryViewPanel groceryViewPanel;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text purchaseButtonText;
    [SerializeField] private GameObject goldPanel;
    [SerializeField] private TMP_Text goldValueText;
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private TMP_Text lockText;
    [SerializeField] private GameObject lockOverlay;

    private StageType selectedStage = StageType.Count;
    private UpgradeManager subscribedUpgradeManager;
    private StockManager subscribedStockManager;
    private MarketManager subscribedMarketManager;
    private bool isInitialized;

    public void Initialize()
    {
        if (isInitialized)
            return;

        isInitialized = true;
        purchaseButton?.onClick.AddListener(OnClickPurchase);

        subscribedUpgradeManager = GameManager.Instance?.Upgrade;
        subscribedStockManager = GameManager.Instance?.StockManager;
        subscribedMarketManager = GameManager.Instance?.Market;

        subscribedUpgradeManager?.SubscribeUpgradeChanged(Refresh);
        subscribedStockManager?.SubscribeStockDataChange(Refresh);
        subscribedMarketManager?.SubscribeMarketDataChanged(Refresh);
    }

    public void Show(StageType stageType)
    {
        int stageIndex = (int)stageType;

        if (stageIndex < 0 || stageIndex >= (int)StageType.Count)
            return;

        selectedStage = stageType;
        Refresh();
    }

    public void Refresh()
    {
        if (selectedStage == StageType.Count)
            return;

        StageDataSO stageData = StageDataDB.GetData(selectedStage);

        if (stageNameText != null)
        {
            stageNameText.text = stageData != null
                ? stageData.DisplayName
                : selectedStage.ToString();
        }

        if (stageDescriptionText != null)
            stageDescriptionText.text = stageData?.Description ?? string.Empty;

        if (stageIcon != null)
            stageIcon.sprite = stageData?.Icon;

        groceryViewPanel?.Initialize(stageData?.RewardList);
        RefreshPurchaseButton();
    }

    private void OnDestroy()
    {
        purchaseButton?.onClick.RemoveListener(OnClickPurchase);
        subscribedUpgradeManager?.UnsubscribeUpgradeChanged(Refresh);
        subscribedStockManager?.UnsubscribeStockDataChange(Refresh);
        subscribedMarketManager?.UnsubscribeMarketDataChanged(Refresh);
    }

    private void RefreshPurchaseButton()
    {
        if (purchaseButton == null || purchaseButtonText == null)
            return;

        UpgradeManager upgradeManager = GameManager.Instance?.Upgrade;

        if (upgradeManager == null)
        {
            SetPurchaseButton(false, false, "데이터 오류");
            return;
        }

        int currentLevel = upgradeManager.RuntimeLevel.Get(
            HarvestUpgradeType.StageLevel);
        int requiredLevel = (int)selectedStage + 1;

        if (currentLevel >= requiredLevel)
        {
            SetPurchaseButton(false, false, "구매됨");
            return;
        }

        if (requiredLevel != currentLevel + 1)
        {
            SetPurchaseButton(false, false, "이전 스테이지 필요");
            return;
        }

        HarvestUpgradeDataSO upgradeData = UpgradeDataDB.GetData(
            HarvestUpgradeType.StageLevel);

        if (upgradeData == null)
        {
            SetPurchaseButton(false, false, "데이터 오류");
            return;
        }

        UpgradeAvailability availability =
            upgradeManager.GetUpgradeAvailability(upgradeData);

        bool showGoldPanel =
            availability == UpgradeAvailability.Available ||
            availability == UpgradeAvailability.InsufficientCurrency;

        string conditionText = GetPurchaseButtonText(
            upgradeData,
            currentLevel,
            availability);

        if (showGoldPanel
            && upgradeData.TryGetRequiredCost(currentLevel + 1, out int cost))
        {
            conditionText = cost.ToString("N0");
        }

        SetPurchaseButton(
            availability == UpgradeAvailability.Available,
            showGoldPanel,
            conditionText);
    }

    private void OnClickPurchase()
    {
        UpgradeManager upgradeManager = GameManager.Instance?.Upgrade;

        if (upgradeManager == null || selectedStage == StageType.Count)
            return;

        int currentLevel = upgradeManager.RuntimeLevel.Get(
            HarvestUpgradeType.StageLevel);
        int requiredLevel = (int)selectedStage + 1;

        if (requiredLevel != currentLevel + 1)
            return;

        HarvestUpgradeDataSO upgradeData = UpgradeDataDB.GetData(
            HarvestUpgradeType.StageLevel);

        if (upgradeData != null)
            upgradeManager.TryUpgrade(upgradeData);
    }

    private void SetPurchaseButton(
        bool interactable,
        bool showGoldPanel,
        string conditionText)
    {
        purchaseButton.interactable = interactable;
        purchaseButtonText.text = "개방하기";
        lockOverlay.SetActive(!interactable);
        goldPanel.SetActive(showGoldPanel);
        lockPanel.SetActive(!showGoldPanel);

        if (showGoldPanel)
            goldValueText.text = conditionText;
        else
            lockText.text = conditionText;
    }

    private static string GetPurchaseButtonText(
        HarvestUpgradeDataSO upgradeData,
        int currentLevel,
        UpgradeAvailability availability)
    {
        if (availability == UpgradeAvailability.MarketLevelLocked
            && upgradeData.TryGetRequiredMarketLevel(
                currentLevel + 1,
                out int requiredMarketLevel))
        {
            return $"마켓 Lv.{requiredMarketLevel} 필요";
        }

        return availability switch
        {
            UpgradeAvailability.Available => "구매",
            UpgradeAvailability.InsufficientCurrency => "재화 부족",
            UpgradeAvailability.MaxLevel => "최대 단계",
            UpgradeAvailability.InvalidData => "데이터 오류",
            _ => "구매 불가"
        };
    }
}
