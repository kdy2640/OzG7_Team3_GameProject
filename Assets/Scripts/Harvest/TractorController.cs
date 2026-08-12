using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class TractorController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string moveActionPath = "Player/Move";
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float rotationLerpSpeed = 120f;
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
        float throttle = moveInput.y;
        float steering = moveInput.x;
        bool isMoving = Mathf.Abs(throttle) > 0.0001f;

        if (!isMoving && Mathf.Abs(steering) <= 0.0001f)
        {
            return;
        }

        float speedMultiplier =
            cropCutter == null ? 1f : cropCutter.MoveSpeedMultiplier;

        float steeringAngle =
            steering
            * (isMoving ? throttle : 1f)
            * rotationLerpSpeed
            * Time.fixedDeltaTime;
        Quaternion nextRotation =
            body.rotation * Quaternion.Euler(0f, steeringAngle, 0f);
        body.MoveRotation(nextRotation);

        if (isMoving && speedMultiplier > 0f)
        {
            Vector3 forward = nextRotation * Vector3.forward;
            body.MovePosition(
                body.position
                + forward
                * (throttle * moveSpeed * speedMultiplier * Time.fixedDeltaTime));
        }
    }
}
