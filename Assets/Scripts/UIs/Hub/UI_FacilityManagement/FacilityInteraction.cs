using UnityEngine;

public class FacilityInteraction : MonoBehaviour
{
    [SerializeField] private FacilityDetailPanel detailPanel;

    public void OnFacilityClicked(FacilityController facility)
    {
        if (facility == null || detailPanel == null) return;

        detailPanel.ShowFacility(facility);
    }
}