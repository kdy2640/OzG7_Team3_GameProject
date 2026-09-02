using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class FacilityDetailPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text facilityNameText;
    [SerializeField] private UI_FacilityLevelDisplay currentLevelDisplay;
    [SerializeField] private UI_FacilityLevelDisplay nextLevelDisplay;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;
    [SerializeField] private GameObject unavailableButton;
    [SerializeField] private TMP_Text unavailableButtonText;
    [SerializeField] private GameObject goldStruct;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private PanelAnimator panelAnimator;

    private FacilityCollection facilityCollection;
    private FacilityType currentFacilityType = FacilityType.Count;
    private UpgradeManager subscribedUpgradeManager;
    private StockManager subscribedStockManager;
    private MarketManager subscribedMarketManager;

    private void Awake()
    {
        actionButton.onClick.AddListener(OnClickAction);
        previousButton.onClick.AddListener(OnClickPrevious);
        nextButton.onClick.AddListener(OnClickNext);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        subscribedUpgradeManager = GameManager.Instance.Upgrade;
        subscribedStockManager = GameManager.Instance.StockManager;
        subscribedMarketManager = GameManager.Instance.Market;

        subscribedUpgradeManager.SubscribeUpgradeChanged(Refresh);
        subscribedStockManager.SubscribeStockDataChange(Refresh);
        subscribedMarketManager.SubscribeMarketDataChanged(Refresh);

        Refresh();
    }

    private void OnDisable()
    {
        if (subscribedUpgradeManager != null)
            subscribedUpgradeManager.UnsubscribeUpgradeChanged(Refresh);

        if (subscribedStockManager != null)
            subscribedStockManager.UnsubscribeStockDataChange(Refresh);

        if (subscribedMarketManager != null)
            subscribedMarketManager.UnsubscribeMarketDataChanged(Refresh);

        subscribedUpgradeManager = null;
        subscribedStockManager = null;
        subscribedMarketManager = null;
    }

    public void Initialize(FacilityCollection collection)
    {
        facilityCollection = collection;
    }

    public IEnumerator ShowFacility(FacilityType facilityType)
    {
        if (facilityType == FacilityType.Count)
            yield break;

        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_Select);

        bool wasActive = gameObject.activeSelf;

        currentFacilityType = facilityType;
        gameObject.SetActive(true);
        Refresh();

        if (!wasActive)
            yield return panelAnimator.Show();
    }

    public void ClosePanel()
    {
        currentFacilityType = FacilityType.Count;
        facilityCollection.ClearSelection();
        gameObject.SetActive(false);
    }

    private void OnClickAction()
    {
        if (currentFacilityType == FacilityType.Count)
            return;

        FacilityUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(currentFacilityType);

        if (upgradeData == null)
            return;

        GameManager.Instance.Upgrade.TryUpgrade(upgradeData);
    }

    private void OnClickPrevious()
    {
        if (facilityCollection.TryGetPrevious(
                currentFacilityType,
                out FacilityType previous))
        {
            facilityCollection.ShowDetail(previous);
        }
    }

    private void OnClickNext()
    {
        if (facilityCollection.TryGetNext(
                currentFacilityType,
                out FacilityType next))
        {
            facilityCollection.ShowDetail(next);
        }
    }

    private void Refresh()
    {
        if (currentFacilityType == FacilityType.Count)
            return;

        FacilityDataSO facilityData =
            FacilityDataDB.GetData(currentFacilityType);
        FacilityUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(currentFacilityType);
        UpgradeManager upgradeManager = GameManager.Instance.Upgrade;

        if (facilityData == null || upgradeData == null)
        {
            RefreshInvalidData();
            RefreshNavigationButtons();
            return;
        }

        int currentLevel =
            upgradeManager.RuntimeLevel.Get(currentFacilityType);
        bool isMaxLevel = currentLevel >= upgradeData.MaxLevel;
        UpgradeAvailability availability =
            upgradeManager.GetUpgradeAvailability(upgradeData);

        facilityNameText.text = facilityData.DisplayName;

        currentLevelDisplay.SetData(
            currentLevel,
            GetLevelEffect(facilityData, currentLevel));
        currentLevelDisplay.SetMaxLevel(isMaxLevel);

        nextLevelDisplay.gameObject.SetActive(!isMaxLevel);

        if (!isMaxLevel)
        {
            nextLevelDisplay.SetData(
                currentLevel + 1,
                GetLevelEffect(facilityData, currentLevel + 1));
        }
        bool showCurrencyAction =
            availability == UpgradeAvailability.Available ||
            availability == UpgradeAvailability.InsufficientCurrency;

        costText.text = showCurrencyAction
            && upgradeData.TryGetRequiredCost(currentLevel + 1, out int cost)
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
                GetUnavailableText(upgradeData, currentLevel, availability);
        }

        RefreshNavigationButtons();
    }

    private void RefreshInvalidData()
    {
        facilityNameText.text = currentFacilityType.ToString();
        currentLevelDisplay.SetInvalidData();
        nextLevelDisplay.gameObject.SetActive(true);
        nextLevelDisplay.SetInvalidData();
        costText.text = "-";
        actionButton.gameObject.SetActive(false);
        unavailableButton.SetActive(true);
        unavailableButtonText.text = "데이터 오류";
        goldStruct.SetActive(false);
    }

    private void RefreshNavigationButtons()
    {
        bool hasPrevious = facilityCollection.TryGetPrevious(
            currentFacilityType,
            out _);
        bool hasNext = facilityCollection.TryGetNext(
            currentFacilityType,
            out _);

        previousButton.gameObject.SetActive(hasPrevious);
        nextButton.gameObject.SetActive(hasNext);
    }

    private static string GetLevelEffect(
        FacilityDataSO facilityData,
        int level)
    {
        if (level <= 0)
            return "구매 전";

        string effect = level switch
        {
            1 => facilityData.Level1Skill,
            2 => facilityData.Level2Skill,
            3 => facilityData.Level3Skill,
            4 => facilityData.Level4Skill,
            5 => facilityData.Level5Skill,
            _ => string.Empty
        };

        return string.IsNullOrWhiteSpace(effect)
            ? "효과 정보 없음"
            : effect;
    }

    private static string GetUnavailableText(
        FacilityUpgradeDataSO upgradeData,
        int currentLevel,
        UpgradeAvailability availability)
    {
        if (availability == UpgradeAvailability.MarketLevelLocked
            && upgradeData.TryGetRequiredMarketLevel(
                currentLevel + 1,
                out int requiredMarketLevel))
        {
            return $"레스토랑 레벨 {requiredMarketLevel}에 잠금 해제";
        }

        return availability switch
        {
            UpgradeAvailability.MaxLevel => "최대 레벨",
            UpgradeAvailability.InvalidData => "데이터 오류",
            _ => "이용 불가"
        };
    }
}
