using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FacilityRaycaster : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private LayerMask clickableLayer; // 클릭할 레이어를 Inspector에서 지정하세요.
    [SerializeField] private float maxDistance = 1000f;

    private readonly List<RaycastResult> uiRaycastResults = new();

    private void Awake()
    {
        EnsureCamera();
    }

    private void Update()
    {
        // 마우스 또는 터치 포인터 검사
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame) return;

        EnsureCamera();
        if (targetCamera == null) return;

        Vector2 mousePos = Pointer.current.position.ReadValue();

        if (TryGetFacilityFromUI(mousePos, out FacilityClickTarget uiTarget))
        {
            uiTarget.OnClicked();
            return;
        }

        // ScreenPointToRay 생성
        Ray ray = targetCamera.ScreenPointToRay(mousePos);

        // LayerMask와 MaxDistance를 적용하여 Raycast
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, clickableLayer))
        {
            FacilityClickTarget target = hit.collider.GetComponent<FacilityClickTarget>();
            target?.OnClicked();
        }
    }

    private bool TryGetFacilityFromUI(
        Vector2 screenPosition,
        out FacilityClickTarget target)
    {
        target = null;
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new(EventSystem.current)
        {
            position = screenPosition
        };

        uiRaycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, uiRaycastResults);

        foreach (RaycastResult result in uiRaycastResults)
        {
            target = result.gameObject.GetComponentInParent<FacilityClickTarget>();
            if (target != null) return true;
        }

        return false;
    }

    private void EnsureCamera()
    {
        if (targetCamera == null || !targetCamera.gameObject.activeInHierarchy)
        {
            targetCamera = Camera.main;
        }
    }
}
