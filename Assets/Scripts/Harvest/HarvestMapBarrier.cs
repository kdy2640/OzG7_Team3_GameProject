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
    [SerializeField, Min(0f)] private float returnBuffer = 0.5f;

    private MaterialPropertyBlock propertyBlock;
    private Rigidbody tractorBody;
    private Collider tractorCollider;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        tractorBody = tractor.GetComponent<Rigidbody>();
        tractorCollider = tractor.GetComponent<Collider>();
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

    private void OnTriggerEnter(Collider other)
    {
        TryReturnTractor(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryReturnTractor(other);
    }

    private void TryReturnTractor(Collider other)
    {
        if (other != tractorCollider)
            return;

        Bounds barrierBounds = barrierRenderer.bounds;
        Vector3 barrierCenter = barrierBounds.center;
        Vector3 toMapCenter = transform.parent.position - barrierCenter;
        toMapCenter.y = 0f;

        bool blocksXAxis =
            Mathf.Abs(toMapCenter.x) > Mathf.Abs(toMapCenter.z);
        Vector3 insideDirection = blocksXAxis
            ? new Vector3(Mathf.Sign(toMapCenter.x), 0f, 0f)
            : new Vector3(0f, 0f, Mathf.Sign(toMapCenter.z));
        float signedDistance = Vector3.Dot(
            tractorBody.position - barrierCenter,
            insideDirection);

        if (signedDistance >= 0f)
            return;

        Bounds tractorBounds = tractorCollider.bounds;
        float barrierExtent = blocksXAxis
            ? barrierBounds.extents.x
            : barrierBounds.extents.z;
        float tractorExtent = blocksXAxis
            ? tractorBounds.extents.x
            : tractorBounds.extents.z;
        float returnDistance =
            barrierExtent + tractorExtent + returnBuffer;
        Vector3 returnPosition = tractorBody.position;

        if (blocksXAxis)
        {
            returnPosition.x = barrierCenter.x
                + insideDirection.x * returnDistance;
        }
        else
        {
            returnPosition.z = barrierCenter.z
                + insideDirection.z * returnDistance;
        }

        tractorBody.position = returnPosition;

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
        returnBuffer = Mathf.Max(0f, returnBuffer);
    }
}
