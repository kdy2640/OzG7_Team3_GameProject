using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HarvestUpgradeDetailPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private UI_FacilityLevelDisplay currentLevelDisplay;
    [SerializeField] private UI_FacilityLevelDisplay nextLevelDisplay;
    [SerializeField] private Slider levelSlider;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;
    [SerializeField] private GameObject unavailableButton;
    [SerializeField] private TMP_Text unavailableButtonText;
    [SerializeField] private Button closeButton;
    [SerializeField] private PanelAnimator panelAnimator;
    [SerializeField] private GameObject goldStruct;

    private HarvestUpgradeType currentUpgradeType = HarvestUpgradeType.Count;
    private UpgradeManager subscribedUpgradeManager;
    private StockManager subscribedStockManager;
    private MarketManager subscribedMarketManager;

    private void Awake()
    {
        actionButton?.onClick.AddListener(OnClickUpgrade);
        closeButton?.onClick.AddListener(ClosePanel);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        subscribedUpgradeManager = GameManager.Instance?.Upgrade;
        subscribedStockManager = GameManager.Instance?.StockManager;
        subscribedMarketManager = GameManager.Instance?.Market;

        subscribedUpgradeManager?.SubscribeUpgradeChanged(Refresh);
        subscribedStockManager?.SubscribeStockDataChange(Refresh);
        subscribedMarketManager?.SubscribeMarketDataChanged(Refresh);

        Refresh();
    }

    private void OnDisable()
    {
        subscribedUpgradeManager?.UnsubscribeUpgradeChanged(Refresh);
        subscribedStockManager?.UnsubscribeStockDataChange(Refresh);
        subscribedMarketManager?.UnsubscribeMarketDataChanged(Refresh);

        subscribedUpgradeManager = null;
        subscribedStockManager = null;
        subscribedMarketManager = null;
    }

    public IEnumerator ShowUpgrade(HarvestUpgradeType upgradeType)
    {
        if (upgradeType == HarvestUpgradeType.Count)
            yield break;

        bool wasActive = gameObject.activeSelf;

        currentUpgradeType = upgradeType;
        gameObject.SetActive(true);
        Refresh();

        if (!wasActive)
            yield return panelAnimator.Show();
    }

    public IEnumerator HidePanel()
    {
        if (!gameObject.activeSelf)
            yield break;

        currentUpgradeType = HarvestUpgradeType.Count;
        yield return panelAnimator.Hide();
        gameObject.SetActive(false);
    }

    public void ClosePanel()
    {
        currentUpgradeType = HarvestUpgradeType.Count;
        gameObject.SetActive(false);
    }

    private void OnClickUpgrade()
    {
        if (currentUpgradeType == HarvestUpgradeType.Count)
            return;

        HarvestUpgradeDataSO data = UpgradeDataDB.GetData(currentUpgradeType);

        if (data == null)
            return;

        GameManager.Instance?.Upgrade?.TryUpgrade(data);
    }

    private void Refresh()
    {
        if (currentUpgradeType == HarvestUpgradeType.Count)
            return;

        HarvestUpgradeDataSO data = UpgradeDataDB.GetData(currentUpgradeType);
        UpgradeManager upgradeManager = GameManager.Instance?.Upgrade;

        if (data == null || upgradeManager == null)
        {
            RefreshInvalidData();
            return;
        }

        int currentLevel = upgradeManager.RuntimeLevel.Get(currentUpgradeType);
        bool isMaxLevel = currentLevel >= data.MaxLevel;
        UpgradeAvailability availability =
            upgradeManager.GetUpgradeAvailability(data);

        if (upgradeNameText != null)
            upgradeNameText.text = data.DisplayName;

        currentLevelDisplay.SetData(
            currentLevel,
            data.MaxLevel,
            BuildEffectText(data, currentLevel));
        currentLevelDisplay.SetMaxLevel(isMaxLevel);

        nextLevelDisplay.gameObject.SetActive(!isMaxLevel);

        if (!isMaxLevel)
        {
            nextLevelDisplay.SetData(
                currentLevel + 1,
                data.MaxLevel,
                BuildEffectText(data, currentLevel + 1));
        }

        if (levelSlider != null)
        {
            levelSlider.maxValue = Mathf.Max(1, data.MaxLevel);
            levelSlider.value = currentLevel;
        }

        bool showCurrencyAction =
            availability == UpgradeAvailability.Available ||
            availability == UpgradeAvailability.InsufficientCurrency;

        costText.text = showCurrencyAction
            && data.TryGetRequiredCost(currentLevel + 1, out int cost)
                ? cost.ToString()
                : "-";

        actionButton.gameObject.SetActive(showCurrencyAction);
        unavailableButton.SetActive(!showCurrencyAction);
        goldStruct.SetActive(showCurrencyAction);

        actionButton.interactable =
            availability == UpgradeAvailability.Available;

        if (showCurrencyAction)
        {
            actionButtonText.text =
                availability == UpgradeAvailability.InsufficientCurrency
                    ? "자금 부족"
                    : currentLevel <= 0 ? "구매" : "업그레이드";
        }
        else
        {
            unavailableButtonText.text =
                GetUnavailableText(data, currentLevel, availability);
        }
    }

    private void RefreshInvalidData()
    {
        if (upgradeNameText != null)
            upgradeNameText.text = currentUpgradeType.ToString();

        currentLevelDisplay.SetInvalidData();
        nextLevelDisplay.gameObject.SetActive(true);
        nextLevelDisplay.SetInvalidData();
        actionButton.gameObject.SetActive(false);
        unavailableButton.SetActive(true);
        unavailableButtonText.text = "데이터 오류";
        goldStruct.SetActive(false);

        if (costText != null)
            costText.text = "-";

        if (actionButton != null)
            actionButton.interactable = false;

    }

    private static string BuildEffectText(
        HarvestUpgradeDataSO data,
        int level)
    {
        if (level <= 0)
            return "업그레이드 전";

        StringBuilder builder = new();

        for (int i = 0; i < data.StatModifiers.Count; i++)
        {
            HarvestStatModifier modifier = data.StatModifiers[i];

            if (builder.Length > 0)
                builder.AppendLine();

            float value = modifier.value * level;
            builder.Append(modifier.statType switch
            {
                HarvestStatType.SawSize => "톱날 크기",
                HarvestStatType.SawSpeed => "톱날 속도",
                HarvestStatType.SawSharpness => "톱날 날카로움",
                HarvestStatType.TruckSpeed => "트럭 속도",
                HarvestStatType.TruckCapacity => "트럭 적재량",
                HarvestStatType.TruckFuel => "트럭 연료",
                HarvestStatType.GoldenPigDetectionRadius => "황금돼지 탐지 반경",
                _ => modifier.statType.ToString()
            });

            switch (modifier.modifierType)
            {
                case ModifierType.Multiply:
                    builder.Append($" +{value * 100f:0.##}%");
                    break;

                default:
                    builder.Append($" +{value:0.##}");
                    break;
            }
        }

        return builder.Length > 0 ? builder.ToString() : "효과 정보 없음";
    }

    private static string GetUnavailableText(
        HarvestUpgradeDataSO data,
        int currentLevel,
        UpgradeAvailability availability)
    {
        if (availability == UpgradeAvailability.MarketLevelLocked
            && data.TryGetRequiredMarketLevel(
                currentLevel + 1,
                out int requiredMarketLevel))
        {
            return $"레스토랑레벨 {requiredMarketLevel}에 잠금해제.";
        }

        return availability switch
        {
            UpgradeAvailability.MaxLevel => "최대 레벨",
            UpgradeAvailability.InvalidData => "데이터 오류",
            _ => "이용 불가"
        };
    }
}
