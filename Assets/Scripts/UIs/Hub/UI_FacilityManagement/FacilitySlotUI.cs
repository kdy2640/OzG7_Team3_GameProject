using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilitySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text facilityNameText;
    [SerializeField] private TMP_Text levelText;

    [Header("Facility")]
    [SerializeField] private FacilityController facility;

    [Header("Detail Panel")]
    [SerializeField] private FacilityDetailPanel detailPanel;


    private void Start()
    {
        Refresh();
    }


    public void Refresh()
    {
        if (facility == null) return;

        facilityNameText.text = facility.FacilityName;

        if (facility.IsPurchased)
        {
            levelText.text = $"Lv.{facility.CurrentLevel}";
        }
        else
        {
            levelText.text = "LOCKED";
        }
    }


    public void OnClickSlot()
    {
        if (facility == null) return;
        
        if (detailPanel == null) return;
        
        detailPanel.ShowFacility(facility);
    }
}