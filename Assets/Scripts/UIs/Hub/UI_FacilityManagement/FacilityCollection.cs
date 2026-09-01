using System;
using UnityEngine;

public class FacilityCollection : MonoBehaviour
{
    private FacilityController[] facilities;

    public event Action<FacilityType> FacilitySelected;

    private void Awake()
    {
        facilities = GetComponentsInChildren<FacilityController>(true);
    }

    public void ShowDetail(FacilityType facilityType)
    {
        if (facilityType == FacilityType.Count) return;

        FacilitySelected?.Invoke(facilityType);
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
