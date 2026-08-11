using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FacilityRaycaster : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // UI 버튼을 클릭한 경우 월드 클릭 처리하지 않음
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        Ray ray = targetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        FacilityClickTarget target =
            hit.collider.GetComponentInParent<FacilityClickTarget>();

        target?.OnClicked();
    }
}