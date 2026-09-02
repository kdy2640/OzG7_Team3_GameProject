using UnityEngine;

public class FacilityModelView : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private FacilityController facility;

    [Header("Model")]
    [Tooltip("���� ���� ���� ������ ��ġ")]
    [SerializeField] private Transform facilityModelRoot;

    private GameObject currentModelInstance;
    private Renderer[] currentModelRenderers;
    private int shownLevel = -1;
    private bool isFacilityUpgradeViewEnabled;
    private bool isSelected;

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (facility == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityController�� ������� �ʾҽ��ϴ�: {name}",
                this );
            return;
        }

        ShowLevel(facility.CurrentLevel);
    }

    public void ShowLocked()
    {
        ShowLevel(0);
    }

    internal void SetFacilityUpgradeView(bool isEnabled)
    {
        if (isFacilityUpgradeViewEnabled == isEnabled)
            return;

        isFacilityUpgradeViewEnabled = isEnabled;
        Refresh();
    }

    public void ShowLevel(int level)
    {
        if (level <= 0 && !isFacilityUpgradeViewEnabled)
        {
            ClearCurrentModel();
            return;
        }

        if (facility == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityController�� ������� �ʾҽ��ϴ�: {name}",
                this );
            return;
        }

        if (facilityModelRoot == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityModelRoot�� ������� �ʾҽ��ϴ�: {name}",
                this );
            return;
        }

        FacilityDataSO facilityData = FacilityDataDB.GetData(facility.FacilityType);

        if (facilityData == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityData�� �����ϴ�: {facility.FacilityType}",
                this );
            return;
        }

        // ���� ������ ���� �̹� �����ϸ� �ٽ� �������� ����
        if (shownLevel == level && currentModelInstance != null)
        {
            return;
        }

        GameObject modelPrefab = facilityData.GetSolidPrefabForLevel(level);

        if (modelPrefab == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] " +
                $"{facilityData.DisplayName}�� Lv.{level} ���� �����ϴ�.",
                this
            );
            return;
        }

        ClearCurrentModel();

        

        currentModelInstance =
            Instantiate( modelPrefab, facilityModelRoot, false );

        currentModelInstance.transform.localPosition = Vector3.zero;

        currentModelInstance.transform.localRotation = Quaternion.identity;

        currentModelInstance.transform.localScale = Vector3.one;

        currentModelRenderers =
            currentModelInstance.GetComponentsInChildren<Renderer>(true);
        ApplyOutlineRenderingLayer();

        shownLevel = level;

    }

    public void PlayUpgradeEffect()
    {
        if (facility != null)
        {
            ShowLevel(facility.CurrentLevel);
        }
    }

    internal void SetSelected(bool selected)
    {
        if (isSelected == selected)
            return;

        isSelected = selected;
        ApplyOutlineRenderingLayer();
    }

    private void ApplyOutlineRenderingLayer()
    {
        if (currentModelRenderers == null)
            return;

        uint outlineMask =
            FacilityOutlineRendererFeature.SelectedRenderingLayerMask;

        foreach (Renderer targetRenderer in currentModelRenderers)
        {
            if (isSelected)
                targetRenderer.renderingLayerMask |= outlineMask;
            else
                targetRenderer.renderingLayerMask &= ~outlineMask;
        }
    }

    private void ClearCurrentModel()
    {
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        currentModelInstance = null;
        currentModelRenderers = null;
        shownLevel = -1;
    }
}
