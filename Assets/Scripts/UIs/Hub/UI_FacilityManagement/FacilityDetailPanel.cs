using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityDetailPanel : MonoBehaviour
{
    [Header("UI References")]
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

    [Header("Facilities")]
    [SerializeField] private FacilityController[] facilities;

    private int currentIndex;
    private FacilityController currentFacility;

    private void Awake()
    {
        actionButton.onClick.AddListener(OnClickAction);
        previousButton.onClick.AddListener(OnClickPrevious);
        nextButton.onClick.AddListener(OnClickNext);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        // 이 오브젝트는 씬 시작 시 Active 상태여야 합니다.
        // Awake에서 한 번 초기화한 후 숨깁니다.
        gameObject.SetActive(false);
    }

    public void ShowFacility(FacilityController facility)
    {
        if (facility == null)  return;

        UnsubscribeCurrentFacility();

        currentFacility = facility;
        currentFacility.StateChanged += OnFacilityStateChanged;

        FindCurrentIndex();

        gameObject.SetActive(true);
        Refresh();
    }

    public void OnClickAction()
    {
        if (currentFacility == null) return;

        if (!currentFacility.IsPurchased)
            currentFacility.TryPurchase();
        else
            currentFacility.TryUpgrade();
    }

    public void OnClickPrevious()
    {
        if (currentIndex <= 0) return;

        ShowFacility(facilities[currentIndex - 1]);
    }

    public void OnClickNext()
    {
        if (facilities == null || currentIndex >= facilities.Length - 1) return;

        ShowFacility(facilities[currentIndex + 1]);
    }

    public void ClosePanel()
    {
        UnsubscribeCurrentFacility();
        currentFacility = null;

        gameObject.SetActive(false);
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
            $"Lv.{currentFacility.CurrentLevel} / Lv.{currentFacility.MaxLevel}";

        levelSlider.maxValue = currentFacility.MaxLevel;
        levelSlider.value = currentFacility.CurrentLevel;

        currentEffectText.text = currentFacility.GetCurrentEffect();
        nextEffectText.text = currentFacility.GetNextEffect();

        RefreshActionButton();
        RefreshNavigationButtons();
    }

    private void RefreshActionButton()
    {
        if (!currentFacility.IsPurchased)
        {
            actionButtonText.text = "Purchase";
            actionButton.interactable = true;
            return;
        }

        if (currentFacility.CanUpgrade())
        {
            actionButtonText.text = "Upgrade";
            actionButton.interactable = true;
            return;
        }

        actionButtonText.text = "Max Level";
        actionButton.interactable = false;
    }

    private void RefreshNavigationButtons()
    {
        bool hasFacilities = facilities != null && facilities.Length > 0;

        previousButton.gameObject.SetActive(hasFacilities && currentIndex > 0);

        nextButton.gameObject.SetActive
            (hasFacilities && currentIndex < facilities.Length - 1);
    }

    private void FindCurrentIndex()
    {
        currentIndex = 0;

        if (facilities == null) return;

        for (int i = 0; i < facilities.Length; i++)
        {
            if (facilities[i] != currentFacility) continue;

            currentIndex = i;
            return;
        }
    }

    private void UnsubscribeCurrentFacility()
    {
        if (currentFacility != null)
            currentFacility.StateChanged -= OnFacilityStateChanged;
    }
}