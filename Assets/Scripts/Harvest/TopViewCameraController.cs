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
            transform.position = target.position + offset;
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
        transform.position = Vector3.Lerp(
            transform.position,
            target.position + offset,
            t);
        transform.rotation = Quaternion.Euler(viewEulerAngles);
    }
}
