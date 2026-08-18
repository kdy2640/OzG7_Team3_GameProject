using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public sealed class HarvestStageBarrier : MonoBehaviour
{
    private static readonly int BaseColorId =
        Shader.PropertyToID("_BaseColor");

    [SerializeField] private Transform tractor;
    [SerializeField] private GridChunkHandler gridChunkHandler;
    [SerializeField] private Renderer barrierRenderer;
    [SerializeField, Min(0f)] private float fadeStartDistance = 20f;
    [SerializeField, Min(0f)] private float fullyVisibleDistance = 7f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.35f;

    private MaterialPropertyBlock propertyBlock;
    private Color barrierColor = Color.red;

    private void Awake()
    {
        barrierRenderer ??= GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        Material material = barrierRenderer.sharedMaterial;

        if (material != null && material.HasProperty(BaseColorId))
        {
            barrierColor = material.GetColor(BaseColorId);
        }
    }

    private void Update()
    { 

        int stageLevel = GameManager.Instance.Upgrade.RuntimeLevel.Get(
            HarvestUpgradeType.StageLevel);

        if (stageLevel >= (int)StageType.Count
            || !StageDataDB.TryGetData(
                (StageType)stageLevel,
                out StageDataSO nextStageData))
        {
            SetVisible(false);
            return;
        }

        Vector3 localPosition = transform.localPosition;
        localPosition.z = nextStageData.ZStart;
        transform.localPosition = localPosition;

        float tractorLocalZ = gridChunkHandler.transform
            .InverseTransformPoint(tractor.position).z;
        float distance = Mathf.Abs(nextStageData.ZStart - tractorLocalZ);
        float visibility = 1f - Mathf.InverseLerp(
            fullyVisibleDistance,
            fadeStartDistance,
            distance);

        SetVisible(visibility > 0f);
        SetAlpha(maxAlpha * visibility);
    }

    private void SetVisible(bool isVisible)
    {
        if (barrierRenderer != null)
        {
            barrierRenderer.enabled = isVisible;
        }
    }

    private void SetAlpha(float alpha)
    {
        barrierColor.a = Mathf.Clamp01(alpha);
        barrierRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, barrierColor);
        barrierRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnValidate()
    {
        fadeStartDistance = Mathf.Max(0f, fadeStartDistance);
        fullyVisibleDistance = Mathf.Clamp(
            fullyVisibleDistance,
            0f,
            fadeStartDistance);
        maxAlpha = Mathf.Clamp01(maxAlpha);
        barrierRenderer ??= GetComponent<Renderer>();
    }
}
