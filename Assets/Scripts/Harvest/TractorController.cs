using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class TractorController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string moveActionPath = "Player/Move";
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float rotationLerpSpeed = 10f;
    [SerializeField] private CropCutter cropCutter;

    private Rigidbody body;
    private InputAction moveAction;
    private Vector2 moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        cropCutter ??= GetComponentInChildren<CropCutter>(true);

        if (inputActions == null)
        {
            Debug.LogError("[TractorController] Input Action Asset is not assigned.", this);
            return;
        }

        moveAction = inputActions.FindAction(moveActionPath, true);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (moveAction == null)
        {
            return;
        }

        moveInput = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);
    }

    private void FixedUpdate()
    {
        Vector3 direction = new(moveInput.x, 0f, moveInput.y);

        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float speedMultiplier =
            cropCutter == null ? 1f : cropCutter.MoveSpeedMultiplier;

        if (speedMultiplier > 0f)
        {
            body.MovePosition(
                body.position
                + direction * (moveSpeed * speedMultiplier * Time.fixedDeltaTime));
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion nextRotation = Quaternion.Lerp(
            body.rotation,
            targetRotation,
            rotationLerpSpeed * Time.fixedDeltaTime);
        body.MoveRotation(nextRotation);
    }
}
