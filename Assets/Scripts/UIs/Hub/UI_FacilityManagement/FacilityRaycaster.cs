using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FacilityRaycaster : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private LayerMask clickableLayer; // 클릭할 레이어를 Inspector에서 지정하세요.
    [SerializeField] private float maxDistance = 1000f;

    private void Awake()
    {
        EnsureCamera();
    }

    private void Update()
    {
        // 마우스 또는 터치 포인터 검사
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame) return;

        // UI 클릭 여부 체크 (New Input System 대응)
        if (IsPointerOverUI()) return;

        EnsureCamera();
        if (targetCamera == null) return;

        // ScreenPointToRay 생성
        Vector2 mousePos = Pointer.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePos);

        // LayerMask와 MaxDistance를 적용하여 Raycast
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, clickableLayer))
        {
            FacilityClickTarget target = hit.collider.GetComponentInParent<FacilityClickTarget>();
            target?.OnClicked();
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // 마우스/포인터 ID를 명시적으로 전달
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void EnsureCamera()
    {
        if (targetCamera == null || !targetCamera.gameObject.activeInHierarchy)
        {
            targetCamera = Camera.main;
        }
    }
}