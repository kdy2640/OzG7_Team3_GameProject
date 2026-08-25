using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HarvestUpgradeDetailPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Slider levelSlider;
    [SerializeField] private Image[] levelSlots = new Image[5];
    [SerializeField] private Color filledSlotColor = new(1f, .78f, .2f);
    [SerializeField] private Color emptySlotColor = new(.3f, .3f, .3f);
    [SerializeField] private TMP_Text currentEffectStateText;
    [SerializeField] private TMP_Text currentEffectText;
    [SerializeField] private GameObject nextEffect;
    [SerializeField] private TMP_Text nextEffectText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;
    [SerializeField] private Button closeButton;
    [SerializeField] private PanelAnimator panelAnimator;

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
        bool isDifferentUpgrade = currentUpgradeType != upgradeType;

        currentUpgradeType = upgradeType;
        gameObject.SetActive(true);
        Refresh();

        if (!wasActive || isDifferentUpgrade)
            yield return panelAnimator.Show();
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

        if (levelText != null)
            levelText.text = $"Lv.{currentLevel} / Lv.{data.MaxLevel}";

        if (levelSlider != null)
        {
            levelSlider.maxValue = Mathf.Max(1, data.MaxLevel);
            levelSlider.value = currentLevel;
        }

        for (int i = 0; i < levelSlots.Length; i++)
        {
            levelSlots[i].color = i < currentLevel
                ? filledSlotColor
                : emptySlotColor;
        }

        currentEffectStateText.text = isMaxLevel ? "만렙" : "현재 효과";
        nextEffect.SetActive(!isMaxLevel);

        if (currentEffectText != null)
            currentEffectText.text = BuildEffectText(data, currentLevel);

        if (nextEffectText != null)
        {
            nextEffectText.text = isMaxLevel
                ? "MAX"
                : BuildEffectText(data, currentLevel + 1);
        }

        if (costText != null)
        {
            costText.text = !isMaxLevel
                && data.TryGetRequiredCost(currentLevel + 1, out int cost)
                    ? cost.ToString()
                    : "-";
        }

        if (actionButton != null)
            actionButton.interactable = availability == UpgradeAvailability.Available;

        if (actionButtonText != null)
            actionButtonText.text = GetAvailabilityText(data, currentLevel, availability);
    }

    private void RefreshInvalidData()
    {
        if (upgradeNameText != null)
            upgradeNameText.text = currentUpgradeType.ToString();

        if (levelText != null)
            levelText.text = "Lv.-";

        if (currentEffectText != null)
            currentEffectText.text = "Data Error";

        if (nextEffectText != null)
            nextEffectText.text = "Data Error";

        if (costText != null)
            costText.text = "-";

        if (actionButton != null)
            actionButton.interactable = false;

        if (actionButtonText != null)
            actionButtonText.text = "Data Error";
    }

    private static string BuildEffectText(
        HarvestUpgradeDataSO data,
        int level)
    {
        if (level <= 0)
            return "Not Upgraded";

        StringBuilder builder = new();

        for (int i = 0; i < data.StatModifiers.Count; i++)
        {
            HarvestStatModifier modifier = data.StatModifiers[i];

            if (builder.Length > 0)
                builder.AppendLine();

            float value = modifier.value * level;
            builder.Append(modifier.statType);

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

        return builder.Length > 0 ? builder.ToString() : "No Effect Data";
    }

    private static string GetAvailabilityText(
        HarvestUpgradeDataSO data,
        int currentLevel,
        UpgradeAvailability availability)
    {
        if (availability == UpgradeAvailability.MarketLevelLocked
            && data.TryGetRequiredMarketLevel(
                currentLevel + 1,
                out int requiredMarketLevel))
        {
            return $"Market Lv.{requiredMarketLevel}";
        }

        return availability switch
        {
            UpgradeAvailability.Available => "Upgrade",
            UpgradeAvailability.MaxLevel => "Max Level",
            UpgradeAvailability.InsufficientCurrency => "Not Enough Currency",
            UpgradeAvailability.InvalidData => "Data Error",
            _ => "Unavailable"
        };
    }
}
