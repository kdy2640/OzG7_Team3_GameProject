using UnityEngine;

public class FacilityClickTarget : MonoBehaviour
{
    [SerializeField] private FacilityController facility;

    private FacilityCollection collection;

    private void Awake()
    {
        if (facility == null) facility = GetComponent<FacilityController>();

        collection = GetComponentInParent<FacilityCollection>();
    }

    public void OnClicked()
    {
        if (facility == null || collection == null) return;

        collection.ShowDetail(facility);
    }
}