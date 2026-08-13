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

    [Header("Source")]
    [SerializeField] private FacilityCollection facilityCollection;

    private FacilityController currentFacility;

    private void Awake()
    {
        actionButton.onClick.AddListener(OnClickAction);
        previousButton.onClick.AddListener(OnClickPrevious);
        nextButton.onClick.AddListener(OnClickNext);

        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);

        gameObject.SetActive(false);
    }

    public void ShowFacility(FacilityController facility)
    {
        if (facility == null) return;

        UnsubscribeCurrentFacility();

        currentFacility = facility;
        currentFacility.StateChanged += OnFacilityStateChanged;

        gameObject.SetActive(true);

        Refresh();
    }

    public void ClosePanel()
    {
        UnsubscribeCurrentFacility();

        currentFacility = null;

        gameObject.SetActive(false);
    }

    private void OnClickAction()
    {
        if (currentFacility == null) return;

        bool success;

        if (!currentFacility.IsPurchased) success = currentFacility.TryPurchase();
        
        else success = currentFacility.TryUpgrade();

        if (success) Refresh();
    }

    private void OnClickPrevious()
    {
        if (currentFacility == null) return;

        FacilityController previous = facilityCollection.GetPrevious(currentFacility);

        if (previous != null) ShowFacility(previous);
    }

    private void OnClickNext()
    {
        if (currentFacility == null) return;

        FacilityController next = facilityCollection.GetNext(currentFacility);

        if (next != null) ShowFacility(next);
    }

    private void OnFacilityStateChanged(FacilityController facility)
    {
        if (facility == currentFacility)
            Refresh();
    }

    private void Refresh()
    {
        if (currentFacility == null) return;

        facilityNameText.text = currentFacility.FacilityName;

        levelText.text =
            $"Lv.{currentFacility.CurrentLevel} / " +
            $"Lv.{currentFacility.MaxLevel}";

        levelSlider.maxValue = currentFacility.MaxLevel;

        levelSlider.value = currentFacility.CurrentLevel;

        currentEffectText.text = currentFacility.GetCurrentEffect();

        nextEffectText.text = currentFacility.GetNextEffect();

        RefreshActionButton();

        previousButton.gameObject.SetActive(
            facilityCollection.GetPrevious(currentFacility) != null);

        nextButton.gameObject.SetActive(
            facilityCollection.GetNext(currentFacility) != null);
    }

    private void RefreshActionButton()
    {
        if (currentFacility == null) return;

        UpgradeAvailability availability = currentFacility.GetUpgradeAvailability();

        if (!currentFacility.IsPurchased)
        {
            actionButtonText.text =
                availability == UpgradeAvailability.Available
                    ? "Purchase" : GetAvailabilityText(availability);

            actionButton.interactable = availability == UpgradeAvailability.Available;

            return;
        }

        actionButtonText.text = GetAvailabilityText(availability);

        actionButton.interactable = availability == UpgradeAvailability.Available;
    }

    private string GetAvailabilityText(UpgradeAvailability availability)
    {
        return availability switch
        {
            UpgradeAvailability.Available =>
                currentFacility.IsPurchased ? "Upgrade" : "Purchase",

            UpgradeAvailability.MaxLevel => "Max Level",

            UpgradeAvailability.MarketLevelLocked => "Market Locked",

            UpgradeAvailability.InsufficientCurrency => "Not Enough Currency",

            UpgradeAvailability.InvalidData => "Data Error",
            
            _ => "Unavailable"
        };
    }

    private void UnsubscribeCurrentFacility()
    {
        if (currentFacility == null) return;

        currentFacility.StateChanged -= OnFacilityStateChanged;
    }
}