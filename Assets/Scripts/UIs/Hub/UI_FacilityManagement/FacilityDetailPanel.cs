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

        if (!currentFacility.IsPurchased) currentFacility.TryPurchase();
        else
            currentFacility.TryUpgrade();
    }

    private void OnClickPrevious()
    {
        FacilityController previous = facilityCollection.GetPrevious(currentFacility);

        if (previous != null) ShowFacility(previous);
    }

    private void OnClickNext()
    {
        FacilityController next =
            facilityCollection.GetNext(currentFacility);

        if (next != null) ShowFacility(next);
    }

    private void OnFacilityStateChanged(FacilityController facility)
    {
        if (facility == currentFacility)
            Refresh();
    }

    private void Refresh()
    {
        if (currentFacility == null)
            return;

        facilityNameText.text = currentFacility.FacilityName;

        levelText.text =
            $"Lv.{currentFacility.CurrentLevel} / Lv.{currentFacility.MaxLevel}";

        levelSlider.maxValue = currentFacility.MaxLevel;
        levelSlider.value = currentFacility.CurrentLevel;

        currentEffectText.text = currentFacility.GetCurrentEffect();
        nextEffectText.text = currentFacility.GetNextEffect();

        bool canAction =
            !currentFacility.IsPurchased || currentFacility.CanUpgrade();

        actionButton.interactable = canAction;

        actionButtonText.text = !currentFacility.IsPurchased
            ? "Purchase" : currentFacility.CanUpgrade()
                ? "Upgrade" : "Max Level";

        previousButton.gameObject.SetActive(
            facilityCollection.GetPrevious(currentFacility) != null);

        nextButton.gameObject.SetActive(
            facilityCollection.GetNext(currentFacility) != null);
    }

    private void UnsubscribeCurrentFacility()
    {
        if (currentFacility != null)
            currentFacility.StateChanged -= OnFacilityStateChanged;
    }
}