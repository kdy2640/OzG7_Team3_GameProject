using UnityEngine;

[DisallowMultipleComponent]
public sealed class CutterViewer : MonoBehaviour
{
    [SerializeField] GameObject cutterVisuals;
    [SerializeField, Min(0f)] private float scaleMultiplier = 1f;
    [SerializeField] private float rotationDegreesPerSecond = 360f;
    [SerializeField, HideInInspector] private float cutterRange;

    public void SetRange(float range)
    {
        cutterRange = Mathf.Max(0f, range);
        ApplyScale();
    }

    private void Update()
    {
        cutterVisuals.transform.Rotate(
            Vector3.left,
            rotationDegreesPerSecond * Time.deltaTime,
            Space.Self);
    }

    private void OnValidate()
    {
        scaleMultiplier = Mathf.Max(0f, scaleMultiplier);
        ApplyScale();
    }

    private void ApplyScale()
    {
        cutterVisuals.transform.localScale =
            Vector3.one * (cutterRange * scaleMultiplier);
    }
}
