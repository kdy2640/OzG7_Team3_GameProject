using System;
using UnityEngine;

public class FacilityCollection : MonoBehaviour
{
    private FacilityController[] facilities;
    private FacilityController selectedFacility;

    public event Action<FacilityType> FacilitySelected;

    private void Awake()
    {
        facilities = GetComponentsInChildren<FacilityController>(true);
    }

    public void ShowDetail(FacilityType facilityType)
    {
        if (facilityType == FacilityType.Count) return;

        FacilityController nextSelection =
            facilities[FindIndex(facilityType)];

        if (selectedFacility != nextSelection)
        {
            if (selectedFacility != null)
                selectedFacility.SetSelected(false);

            selectedFacility = nextSelection;
            selectedFacility.SetSelected(true);
            FacilityOutlineRendererFeature.SetSelectionActive(true);
        }

        FacilitySelected?.Invoke(facilityType);
    }

    public void ClearSelection()
    {
        if (selectedFacility == null)
            return;

        selectedFacility.SetSelected(false);
        selectedFacility = null;
        FacilityOutlineRendererFeature.SetSelectionActive(false);
    }

    public void ShowFirstDetail()
    {
        ShowDetail(FacilityType.Table_1);
    }

    public bool TryGetPrevious(FacilityType current, out FacilityType previous)
    {
        int index = FindIndex(current);

        for (int i = index - 1; i >= 0; i--)
        {
            if (facilities[i] == null) continue;

            previous = facilities[i].FacilityType;
            return true;
        }

        previous = FacilityType.Count;
        return false;
    }

    public bool TryGetNext(FacilityType current, out FacilityType next)
    {
        int index = FindIndex(current);

        for (int i = index + 1; i < facilities.Length; i++)
        {
            if (facilities[i] == null) continue;

            next = facilities[i].FacilityType;
            return true;
        }

        next = FacilityType.Count;
        return false;
    }

    private int FindIndex(FacilityType facilityType)
    {
        for (int i = 0; i < facilities.Length; i++)
        {
            if (facilities[i] != null
                && facilities[i].FacilityType == facilityType)
            {
                return i;
            }
        }

        return -1;
    }
}
