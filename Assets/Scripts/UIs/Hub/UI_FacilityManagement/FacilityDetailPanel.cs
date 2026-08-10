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


    [Header("Facilities")]
    [SerializeField] private FacilityController[] facilities;


    private int currentIndex = 0;

    private FacilityController currentFacility;


    private void Start()
    {
        if (facilities != null && facilities.Length > 0)
        {
            ShowFacility(0);
        }
    }


    public void ShowFacility(int index)
    {
        if (facilities == null || facilities.Length == 0) return;

        if (index < 0 || index >= facilities.Length) return;
        currentIndex = index;

        currentFacility = facilities[currentIndex];

        Refresh();
    }


    public void ShowFacility(FacilityController facility)
    {
        if (facility == null) return;

        currentFacility = facility;

        for (int i = 0; i < facilities.Length; i++)
        {
            if (facilities[i] == facility)
            {
                currentIndex = i;
                break;
            }
        }
        Refresh();
    }

    private void Refresh()
    {
        if (currentFacility == null) return;

        facilityNameText.text = currentFacility.FacilityName;

        levelText.text = $"Lv.{currentFacility.CurrentLevel} / Lv.{currentFacility.MaxLevel}";

        levelSlider.maxValue = currentFacility.MaxLevel;

        levelSlider.value = currentFacility.CurrentLevel;
        
        currentEffectText.text = currentFacility.GetCurrentEffect();

        nextEffectText.text = currentFacility.GetNextEffect();

        RefreshActionButton();

        previousButton.gameObject.SetActive(currentIndex > 0);

        nextButton.gameObject.SetActive(currentIndex < facilities.Length - 1);
    }


    private void RefreshActionButton()
    {
        if (!currentFacility.IsPurchased)
        {
            actionButtonText.text = "Purchase"; return;
        }


        if (currentFacility.CanUpgrade())
        {
            actionButtonText.text = "Upgrade"; return;
        }


        actionButtonText.text = "Max Level";
    }


    public void OnClickAction()
    {
        if (currentFacility == null) return;


        if (!currentFacility.IsPurchased)
        {
            currentFacility.Purchase();
        }
        else
        {
            currentFacility.Upgrade();
        }

        Refresh();
    }


    public void OnClickPrevious()
    {
        if (currentIndex <= 0) return;

        ShowFacility(currentIndex - 1);
    }


    public void OnClickNext()
    {
        if (currentIndex >= facilities.Length - 1) return;

        ShowFacility(currentIndex + 1);
    }
}