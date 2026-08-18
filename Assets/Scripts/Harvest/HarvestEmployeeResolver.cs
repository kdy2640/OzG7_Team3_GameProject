using UnityEngine;

[DisallowMultipleComponent]
public sealed class HarvestEmployeeResolver : MonoBehaviour
{
    [SerializeField] private GridChunkHandler gridChunkHandler;
    [SerializeField] private GameObject harvester1Sidecar;
    [SerializeField] private GameObject harvester2Sidecar;

    private void Start()
    {
        if (gridChunkHandler == null)
        {
            Debug.LogError(
                "[HarvestEmployeeResolver] GridChunkHandler is not assigned.",
                this);
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
        {
            Debug.LogError(
                "[HarvestEmployeeResolver] UpgradeManager is not available.",
                this);
            return;
        }

        ResolveSidecar(EmployeeType.Harvester_1, harvester1Sidecar);
        ResolveSidecar(EmployeeType.Harvester_2, harvester2Sidecar);
    }

    private void ResolveSidecar(EmployeeType employeeType, GameObject sidecar)
    {
        if (sidecar == null)
        {
            Debug.LogError(
                $"[HarvestEmployeeResolver] Sidecar is not assigned. "
                + $"employeeType: {employeeType}",
                this);
            return;
        }

        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(employeeType);
        bool isUnlocked = level > 1;
        sidecar.SetActive(isUnlocked);

        if (!isUnlocked)
        {
            return;
        }

        CropCutter cutter = sidecar.GetComponentInChildren<CropCutter>(true);

        if (cutter == null)
        {
            Debug.LogError(
                $"[HarvestEmployeeResolver] CropCutter was not found. "
                + $"employeeType: {employeeType}",
                sidecar);
            return;
        }

        cutter.Initialize(gridChunkHandler);
        gridChunkHandler.Streamer.AddLoadingTarget(sidecar.transform);
    }
}
