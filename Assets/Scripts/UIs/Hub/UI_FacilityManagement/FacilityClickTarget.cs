using UnityEngine;

public class FacilityClickTarget : MonoBehaviour
{
    [SerializeField] private FacilityController facility;
    [SerializeField] private FacilityInteraction interaction;

    public void OnClicked()
    {
        if (facility == null)
        {
            Debug.LogWarning($"{gameObject.name}: FacilityController가 연결되지 않았습니다.");
            return;
        }

        if (interaction == null)
        {
            Debug.LogWarning($"{gameObject.name}: FacilityInteraction이 연결되지 않았습니다.");
            return;
        }

        interaction.OnFacilityClicked(facility);
    }
}