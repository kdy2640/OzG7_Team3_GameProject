using UnityEngine;

[DisallowMultipleComponent]
public sealed class ItemPresenter : MonoBehaviour
{
    [SerializeField] private float localY;
    [SerializeField, Min(0f)] private float rotationSpeed = 90f;
    [SerializeField, Min(0f)] private float scale = 2f;

    private Transform visualRoot;

    public void Init(GameObject solidPrefab)
    {
        visualRoot = Instantiate(solidPrefab, transform).transform;
        visualRoot.SetLocalPositionAndRotation(
            Vector3.up * localY,
            Quaternion.identity);
        visualRoot.localScale = Vector3.one * scale;
    }

    private void Update()
    {
        visualRoot.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.Self);
    }
}
