using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityDetailPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text facilityNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Slider levelSlider;
    [SerializeField] private TMP_Text currentEffectText;
    [SerializeField] private TMP_Text nextEffectText;

    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;

    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    private FacilityCollection facilityCollection;
    private FacilityType currentFacilityType = FacilityType.Count;

    private UpgradeManager subscribedUpgradeManager;

    private void Awake()
    {
        if (actionButton != null)
            actionButton.onClick.AddListener(OnClickAction);

        if (previousButton != null)
            previousButton.onClick.AddListener(OnClickPrevious);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnClickNext);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        subscribedUpgradeManager =
            GameManager.Instance?.Upgrade;

        subscribedUpgradeManager?.SubscribeUpgradeChanged(
            Refresh
        );

        Refresh();
    }

    private void OnDisable()
    {
        if (subscribedUpgradeManager == null)
            return;

        subscribedUpgradeManager.UnsubscribeUpgradeChanged(
            Refresh
        );

        subscribedUpgradeManager = null;
    }

    public void Initialize(FacilityCollection collection)
    {
        facilityCollection = collection;
    }

    public void ShowFacility(FacilityType facilityType)
    {
        if (facilityType == FacilityType.Count) return;

        currentFacilityType = facilityType;

        gameObject.SetActive(true);

        Refresh();
    }

    public void ClosePanel()
    {
        currentFacilityType = FacilityType.Count;
        gameObject.SetActive(false);
    }

    private void OnClickAction()
    {
        if (currentFacilityType == FacilityType.Count) return;

        UpgradeManager upgradeManager = GameManager.Instance?.Upgrade;

        FacilityUpgradeDataSO upgradeData = UpgradeDataDB.GetData(currentFacilityType);

        if (upgradeManager == null || upgradeData == null) return;

        bool success = upgradeManager.TryUpgrade(upgradeData);

        if (!success) return;

        Refresh();
    }

    private FacilityController FindFacilityController(
        FacilityType facilityType)
    {
        FacilityController[] facilities =
            FindObjectsByType<FacilityController>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (FacilityController facility in facilities)
        {
            if (facility.FacilityType == facilityType)
                return facility;
        }

        return null;
    }

    private void OnClickPrevious()
    {
        if (facilityCollection == null)
            return;

        if (facilityCollection.TryGetPrevious(
                currentFacilityType,
                out FacilityType previous))
        {
            ShowFacility(previous);
        }
    }

    private void OnClickNext()
    {
        if (facilityCollection == null)
            return;

        if (facilityCollection.TryGetNext(
                currentFacilityType,
                out FacilityType next))
        {
            ShowFacility(next);
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

        UpgradeManager upgradeManager =
            GameManager.Instance?.Upgrade;

        int currentLevel =
            upgradeManager != null
                ? upgradeManager.RuntimeLevel.Get(
                    currentFacilityType)
                : 0;

        int maxLevel =
            upgradeData != null
                ? upgradeData.MaxLevel
                : 0;

        bool isPurchased =
            currentLevel > 0;

        UpgradeAvailability availability =
            upgradeManager != null && upgradeData != null
                ? upgradeManager.GetUpgradeAvailability(upgradeData)
                : UpgradeAvailability.InvalidData;

        if (facilityNameText != null)
        {
            facilityNameText.text =
                facilityData != null ? facilityData.DisplayName : currentFacilityType.ToString();
        }

        if (levelText != null)
        {
            levelText.text =
                $"Lv.{currentLevel} / Lv.{maxLevel}";
        }

        if (levelSlider != null)
        {
            levelSlider.maxValue = maxLevel;
            levelSlider.value = currentLevel;
        }

        if (currentEffectText != null)
        {
            currentEffectText.text =
                isPurchased ? $"Lv.{currentLevel}" : "Not Purchased";
        }

        if (nextEffectText != null)
        {
            nextEffectText.text =
                !isPurchased ? "Purchase to unlock"
                    : availability == UpgradeAvailability.Available
                        ? $"Lv.{currentLevel + 1}" : "Max Level";
        }

        RefreshActionButton(
            isPurchased, availability);

        RefreshNavigationButtons();
    }

    private void RefreshActionButton(
        bool isPurchased, UpgradeAvailability availability)
    {
        if (!isPurchased)
        {
            actionButtonText.text =
                availability ==
                    UpgradeAvailability.Available
                    ? "Purchase"
                    : GetAvailabilityText(
                        availability,
                        false );

            actionButton.interactable =
                availability == UpgradeAvailability.Available;

            return;
        }

        actionButtonText.text =
            GetAvailabilityText(
                availability,
                true );

        actionButton.interactable =
            availability == UpgradeAvailability.Available;
    }

    private void RefreshNavigationButtons()
    {
        bool hasPrevious =
            facilityCollection != null &&
            facilityCollection.TryGetPrevious(
                currentFacilityType,
                out _ );

        bool hasNext =
            facilityCollection != null &&
            facilityCollection.TryGetNext(
                currentFacilityType,
                out _ );

        if (previousButton != null)
            previousButton.gameObject.SetActive(
                hasPrevious );

        if (nextButton != null)
            nextButton.gameObject.SetActive(
                hasNext );
    }

    private string GetAvailabilityText(
        UpgradeAvailability availability,
        bool isPurchased)
    {
        return availability switch
        {
            UpgradeAvailability.Available => isPurchased ? "Upgrade" : "Purchase",

            UpgradeAvailability.MaxLevel => "Max Level",

            UpgradeAvailability.MarketLevelLocked => "Market Locked",

            UpgradeAvailability.InsufficientCurrency => "Not Enough Currency",

            UpgradeAvailability.InvalidData => "Data Error",

            _ => "Unavailable"
        };
    }
}