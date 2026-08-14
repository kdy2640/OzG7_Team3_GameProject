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
        actionButton.onClick.AddListener(OnClickAction);
        previousButton.onClick.AddListener(OnClickPrevious);
        nextButton.onClick.AddListener(OnClickNext);

        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        subscribedUpgradeManager = GameManager.Instance?.Upgrade;
        subscribedUpgradeManager?.SubscribeUpgradeChanged(Refresh);
        Refresh();
    }

    private void OnDisable()
    {
        if (subscribedUpgradeManager == null) return;

        subscribedUpgradeManager.UnsubscribeUpgradeChanged(Refresh);
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
        FacilityUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(currentFacilityType);

        if (upgradeManager == null || upgradeData == null) return;

        upgradeManager.TryUpgrade(upgradeData);
    }

    private void OnClickPrevious()
    {
        if (facilityCollection == null) return;

        if (facilityCollection.TryGetPrevious(
                currentFacilityType,
                out FacilityType previous))
        {
            ShowFacility(previous);
        }
    }

    private void OnClickNext()
    {
        if (facilityCollection == null) return;

        if (facilityCollection.TryGetNext(
                currentFacilityType,
                out FacilityType next))
        {
            ShowFacility(next);
        }
    }

    private void Refresh()
    {
        if (currentFacilityType == FacilityType.Count) return;

        FacilityDataSO facilityData =
            FacilityDataDB.GetData(currentFacilityType);
        FacilityUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(currentFacilityType);
        UpgradeManager upgradeManager = GameManager.Instance?.Upgrade;

        int currentLevel = upgradeManager != null
            ? upgradeManager.RuntimeLevel.Get(currentFacilityType)
            : 0;
        int maxLevel = upgradeData != null ? upgradeData.MaxLevel : 0;
        bool isPurchased = currentLevel > 0;

        UpgradeAvailability availability =
            upgradeManager != null && upgradeData != null
                ? upgradeManager.GetUpgradeAvailability(upgradeData)
                : UpgradeAvailability.InvalidData;

        facilityNameText.text = facilityData != null
            ? facilityData.DisplayName
            : currentFacilityType.ToString();

        levelText.text =
            $"Lv.{currentLevel} / " +
            $"Lv.{maxLevel}";

        levelSlider.maxValue = maxLevel;
        levelSlider.value = currentLevel;

        currentEffectText.text = isPurchased
            ? $"Lv.{currentLevel}"
            : "Not Purchased";

        nextEffectText.text = !isPurchased
            ? "Purchase to unlock"
            : availability == UpgradeAvailability.Available
                ? $"Lv.{currentLevel + 1}"
                : "Max Level";

        RefreshActionButton(isPurchased, availability);
        RefreshNavigationButtons();
    }

    private void RefreshActionButton(
        bool isPurchased,
        UpgradeAvailability availability)
    {
        if (!isPurchased)
        {
            actionButtonText.text =
                availability == UpgradeAvailability.Available
                    ? "Purchase"
                    : GetAvailabilityText(availability, false);

            actionButton.interactable =
                availability == UpgradeAvailability.Available;

            return;
        }

        actionButtonText.text = GetAvailabilityText(availability, true);
        actionButton.interactable =
            availability == UpgradeAvailability.Available;
    }

    private void RefreshNavigationButtons()
    {
        bool hasPrevious = facilityCollection != null
            && facilityCollection.TryGetPrevious(currentFacilityType, out _);
        bool hasNext = facilityCollection != null
            && facilityCollection.TryGetNext(currentFacilityType, out _);

        previousButton.gameObject.SetActive(hasPrevious);
        nextButton.gameObject.SetActive(hasNext);
    }

    private string GetAvailabilityText(
        UpgradeAvailability availability,
        bool isPurchased)
    {
        return availability switch
        {
            UpgradeAvailability.Available =>
                isPurchased ? "Upgrade" : "Purchase",

            UpgradeAvailability.MaxLevel => "Max Level",

            UpgradeAvailability.MarketLevelLocked => "Market Locked",

            UpgradeAvailability.InsufficientCurrency => "Not Enough Currency",

            UpgradeAvailability.InvalidData => "Data Error",

            _ => "Unavailable"
        };
    }
}
