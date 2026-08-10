using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class TopViewCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new(0f, 12f, -12f);
    [SerializeField, Min(0f)] private float followSpeed = 8f;
    [SerializeField] private Vector3 viewEulerAngles = new(45f, 0f, 0f);

    private void Start()
    {
        if (target != null)
        {
            Quaternion targetYaw = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
            transform.position = target.position + targetYaw * offset;
            transform.rotation = targetYaw * Quaternion.Euler(viewEulerAngles);
            return;
        }

        transform.rotation = Quaternion.Euler(viewEulerAngles);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        Quaternion targetYaw = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
        Vector3 targetPosition = target.position + targetYaw * offset;
        Quaternion targetRotation =
            targetYaw * Quaternion.Euler(viewEulerAngles);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            t);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            t);
    }
}
