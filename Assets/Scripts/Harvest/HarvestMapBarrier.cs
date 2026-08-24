using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer), typeof(BoxCollider))]
public sealed class HarvestMapBarrier : MonoBehaviour
{
    private static readonly int BaseColorId =
        Shader.PropertyToID("_BaseColor");

    [SerializeField] private Transform tractor;
    [SerializeField] private Renderer barrierRenderer;
    [SerializeField] private BoxCollider barrierCollider;
    [SerializeField, Min(0f)] private float fadeStartDistance = 20f;
    [SerializeField, Min(0f)] private float fullyVisibleDistance = 7f;
    [SerializeField] float FullVisibleAlpha = 0.5f;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        Vector3 closestPoint = barrierCollider.ClosestPoint(tractor.position);
        float distance = Vector3.Distance(tractor.position, closestPoint);
        float visibility = FullVisibleAlpha * (1f - Mathf.InverseLerp(
            fullyVisibleDistance,
            fadeStartDistance,
            distance));

        barrierRenderer.enabled = visibility > 0f;
        SetAlpha(visibility);
    }

    private void SetAlpha(float alpha)
    {
        barrierRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(
            BaseColorId,
            new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
        barrierRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != tractor)
        {
            return;
        }

        GameManager.Instance.Utility.Toast.Show(
            "맵 밖으로 나갈 수 없습니다.");
    }

    private void OnValidate()
    {
        fadeStartDistance = Mathf.Max(0f, fadeStartDistance);
        fullyVisibleDistance = Mathf.Clamp(
            fullyVisibleDistance,
            0f,
            fadeStartDistance);
    }
}
