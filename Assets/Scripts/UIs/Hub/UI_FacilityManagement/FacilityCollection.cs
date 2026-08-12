using UnityEngine;

public class FacilityCollection : MonoBehaviour
{
    [SerializeField] private FacilityDetailPanel detailPanel;

    private FacilityController[] facilities;

    private void Awake()
    {
        facilities = GetComponentsInChildren<FacilityController>(true);
    }

    public void ShowDetail(FacilityController facility)
    {
        if (detailPanel != null) detailPanel.ShowFacility(facility);
    }

    public FacilityController GetPrevious(FacilityController current)
    {
        int index = FindIndex(current);

        for (int i = index - 1; i >= 0; i--)
        {
            if (facilities[i] != null) return facilities[i];
        }
        return null;
    }

    public FacilityController GetNext(FacilityController current)
    {
        int index = FindIndex(current);

        for (int i = index + 1; i < facilities.Length; i++)
        {
            if (facilities[i] != null) return facilities[i];
        }
        return null;
    }

    private int FindIndex(FacilityController facility)
    {
        for (int i = 0; i < facilities.Length; i++)
        {
            if (facilities[i] == facility) return i;
        }
        return -1;
    }
}