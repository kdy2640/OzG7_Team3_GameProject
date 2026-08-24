using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityDetailPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text facilityNameText;
    [SerializeField] private TMP_Text levelText;
    
    [SerializeField] private Transform levelIndicatorRoot;
    [SerializeField] private Image levelIndicatorPrefab;

    [Header("Level Indicator Color")]
    [SerializeField] private Color activeLevelColor = Color.green;
    [SerializeField] private Color inactiveLevelColor = Color.white;

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

    [SerializeField] private PanelAnimator panelAnimator;

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
        subscribedUpgradeManager = GameManager.Instance?.Upgrade;

        subscribedUpgradeManager?.SubscribeUpgradeChanged(Refresh);

        Refresh();
    }

    private void OnDisable()
    {
        if (subscribedUpgradeManager == null)
            return;

        subscribedUpgradeManager.UnsubscribeUpgradeChanged(Refresh);

        subscribedUpgradeManager = null;
    }

    public void Initialize(FacilityCollection collection)
    {
        facilityCollection = collection;
    }

    public IEnumerator ShowFacility(FacilityType facilityType)
    {
        if (facilityType == FacilityType.Count)
            yield break;

        bool wasActive = gameObject.activeSelf;
        bool isDifferentFacility = currentFacilityType != facilityType;

        currentFacilityType = facilityType;

        gameObject.SetActive(true);

        Refresh();

        if (!wasActive || isDifferentFacility)
        {
            yield return panelAnimator.Show();
        }
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


    private void OnClickPrevious()
    {
        if (facilityCollection == null)
            return;

        if (facilityCollection.TryGetPrevious
            (currentFacilityType,out FacilityType previous))
        {
            StartCoroutine(ShowFacility(previous));
        }
    }

    private void OnClickNext()
    {
        if (facilityCollection == null)
            return;

        if (facilityCollection.TryGetNext(currentFacilityType,out FacilityType next))
        {
            StartCoroutine(ShowFacility(next));
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

        int currentLevel = upgradeManager != null
                ? upgradeManager.RuntimeLevel.Get(currentFacilityType) : 0;

        int maxLevel = upgradeData != null
                ? upgradeData.MaxLevel : 0;

        bool isPurchased = currentLevel > 0;

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
            levelText.text = $"Lv.{currentLevel} / Lv.{maxLevel}";
        }

        RefreshLevelIndicators(currentLevel, maxLevel);

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

        RefreshActionButton(isPurchased, availability);

        RefreshNavigationButtons();
    }
    private void RefreshLevelIndicators(int curentLevel, int maxLevel)
    {
        if (levelIndicatorRoot == null || levelIndicatorPrefab == null) return;

        for(int i = levelIndicatorRoot.childCount -1; i >= 0; i--)
        {
            Destroy(levelIndicatorRoot.GetChild(i).gameObject);
        }
        for (int i = 0; i < maxLevel; i++)
        {
            Image indicator = Instantiate(levelIndicatorPrefab, levelIndicatorRoot);

            indicator.color = i<curentLevel ? activeLevelColor : inactiveLevelColor;
        }
    }
    private void RefreshActionButton(bool isPurchased, UpgradeAvailability availability)
    {
        if (!isPurchased)
        {
            actionButtonText.text =
                availability ==
                    UpgradeAvailability.Available ? "Purchase"
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
        bool hasPrevious =
            facilityCollection != null &&
            facilityCollection.TryGetPrevious(currentFacilityType,out _ );

        bool hasNext =
            facilityCollection != null &&
            facilityCollection.TryGetNext(currentFacilityType,out _ );

        if (previousButton != null)
            previousButton.gameObject.SetActive(hasPrevious);

        if (nextButton != null)
            nextButton.gameObject.SetActive(hasNext);
    }

    private string GetAvailabilityText(UpgradeAvailability availability, bool isPurchased)
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
