using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GridChunkHandler))]
public sealed class HarvestSpawner : MonoBehaviour
{
    [SerializeField] private HarvestActor cropPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private StageType stageType = StageType.Stage_1;
    [SerializeField] private Vector2 spawnArea = new(9f, 9f);
    [SerializeField, Min(0.01f)] private float xDiff = 2.5f;
    [SerializeField, Min(0.01f)] private float yDiff = 2.5f;
    [SerializeField] private float spawnHeight;
    [SerializeField] private List<HarvestActor> spawnedCrops = new();

    private Transform cropContainer;
    private GridChunkHandler gridChunkHandler;

    public Vector2 SpawnArea => spawnArea;

    private void Awake()
    {
        gridChunkHandler ??= GetComponent<GridChunkHandler>();
    }

    private void Start()
    {
        Initialize();
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning(
                "[HarvestSpawner] Initialize can only be used in Play Mode.",
                this);
            return;
        }

        if (cropPrefab == null)
        {
            Debug.LogError("[HarvestSpawner] Crop prefab is not assigned.", this);
            return;
        }

        if (!StageDataDB.TryGetData(stageType, out StageDataSO stageData))
        {
            Debug.LogError(
                $"[HarvestSpawner] StageDataSO is not found. stageType : {stageType}",
                this);
            return;
        }

        if (stageData.HarvestList == null || stageData.HarvestList.Count == 0)
        {
            Debug.LogError(
                $"[HarvestSpawner] HarvestList is empty. stageType : {stageType}",
                this);
            return;
        }

        if (gridChunkHandler == null)
        {
            Debug.LogError(
                "[HarvestSpawner] GridChunkHandler is not assigned.",
                this);
            return;
        }

        List<HarvestType> staticTypes = new();
        List<HarvestType> movableTypes = new();

        foreach (HarvestType harvestType in stageData.HarvestList)
        {
            if (!HarvestDataDB.TryGetData(
                    harvestType,
                    out HarvestDataSO harvestData))
            {
                Debug.LogWarning(
                    $"[HarvestSpawner] HarvestDataSO is not found. harvestType : {harvestType}",
                    this);
                continue;
            }

            if (harvestData.IsMove)
            {
                movableTypes.Add(harvestType);
            }
            else
            {
                staticTypes.Add(harvestType);
            }
        }

        if (staticTypes.Count == 0 && movableTypes.Count == 0)
        {
            Debug.LogError(
                $"[HarvestSpawner] There are no valid harvest types. stageType : {stageType}",
                this);
            return;
        }

        float safeXDiff = Mathf.Max(0.01f, xDiff);
        float safeYDiff = Mathf.Max(0.01f, yDiff);
        int xCount = Mathf.Max(1, Mathf.FloorToInt(spawnArea.x / safeXDiff));
        int yCount = Mathf.Max(1, Mathf.FloorToInt(spawnArea.y / safeYDiff));
        int staticCount = staticTypes.Count > 0 ? xCount * yCount : 0;
        float safeChunkSize = gridChunkHandler.ChunkSize;
        Vector2 halfArea = spawnArea * 0.5f;
        int minChunkX = Mathf.FloorToInt(-halfArea.x / safeChunkSize);
        int maxChunkX = Mathf.CeilToInt(halfArea.x / safeChunkSize) - 1;
        int minChunkZ = Mathf.FloorToInt(-halfArea.y / safeChunkSize);
        int maxChunkZ = Mathf.CeilToInt(halfArea.y / safeChunkSize) - 1;
        int chunkXCount = Mathf.Max(0, maxChunkX - minChunkX + 1);
        int chunkZCount = Mathf.Max(0, maxChunkZ - minChunkZ + 1);
        int movableCount = movableTypes.Count > 0
            ? chunkXCount * chunkZCount
            : 0;

        Debug.Log(
            $"[HarvestSpawner] Static : {staticCount}, Movable : {movableCount}, Total : {staticCount + movableCount}",
            this);

        foreach (HarvestActor crop in spawnedCrops)
        {
            if (crop != null)
            {
                gridChunkHandler.Unregister(crop.transform);
                Destroy(crop.gameObject);
            }
        }

        spawnedCrops.Clear();

        if (cropContainer == null)
        {
            cropContainer = transform.Find("Crops");

            if (cropContainer == null)
            {
                cropContainer = new GameObject("Crops").transform;
                cropContainer.SetParent(transform, false);
            }
        }

        float startX = (xCount - 1) * safeXDiff * -0.5f;
        float startY = (yCount - 1) * safeYDiff * -0.5f;

        if (staticTypes.Count == 0)
        {
            Debug.LogWarning(
                $"[HarvestSpawner] Static harvest type is not found. stageType : {stageType}",
                this);
        }
        else
        {
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    Vector3 localPosition = new(
                        startX + x * safeXDiff,
                        spawnHeight,
                        startY + y * safeYDiff);
                    Vector3 worldPosition = transform.TransformPoint(localPosition);
                    HarvestType randomType =
                        staticTypes[Random.Range(0, staticTypes.Count)];

                    HarvestActor crop = Instantiate(
                        cropPrefab,
                        worldPosition,
                        transform.rotation,
                        cropContainer);
                    crop.name = "Crop";
                    crop.Init(randomType, player, this, gridChunkHandler);
                    gridChunkHandler.Register(crop.transform);
                    spawnedCrops.Add(crop);
                }
            }
        }

        if (movableTypes.Count == 0)
        {
            Debug.LogWarning(
                $"[HarvestSpawner] Movable harvest type is not found. stageType : {stageType}",
                this);
            return;
        }

        for (int z = minChunkZ; z <= maxChunkZ; z++)
        {
            for (int x = minChunkX; x <= maxChunkX; x++)
            {
                float chunkMinX = x * safeChunkSize;
                float chunkMaxX = chunkMinX + safeChunkSize;
                float chunkMinZ = z * safeChunkSize;
                float chunkMaxZ = chunkMinZ + safeChunkSize;
                float spawnMinX = Mathf.Max(chunkMinX, -halfArea.x);
                float spawnMaxX = Mathf.Min(chunkMaxX, halfArea.x);
                float spawnMinZ = Mathf.Max(chunkMinZ, -halfArea.y);
                float spawnMaxZ = Mathf.Min(chunkMaxZ, halfArea.y);
                float xInset = Mathf.Min(0.01f, (spawnMaxX - spawnMinX) * 0.25f);
                float zInset = Mathf.Min(0.01f, (spawnMaxZ - spawnMinZ) * 0.25f);
                Vector3 localPosition = new(
                    Random.Range(spawnMinX + xInset, spawnMaxX - xInset),
                    spawnHeight,
                    Random.Range(spawnMinZ + zInset, spawnMaxZ - zInset));
                Vector3 worldPosition = transform.TransformPoint(localPosition);
                HarvestType randomType =
                    movableTypes[Random.Range(0, movableTypes.Count)];

                HarvestActor crop = Instantiate(
                    cropPrefab,
                    worldPosition,
                    transform.rotation,
                    cropContainer);
                crop.name = "Crop";
                crop.Init(randomType, player, this, gridChunkHandler);
                gridChunkHandler.Register(crop.transform);
                spawnedCrops.Add(crop);
            }
        }
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 localPosition = new(
            Random.Range(-spawnArea.x * 0.5f, spawnArea.x * 0.5f),
            spawnHeight,
            Random.Range(-spawnArea.y * 0.5f, spawnArea.y * 0.5f));

        return transform.TransformPoint(localPosition);
    }

    public Vector3 ClampToArea(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        Vector2 halfArea = spawnArea * 0.5f;

        localPosition.x = Mathf.Clamp(localPosition.x, -halfArea.x, halfArea.x);
        localPosition.y = spawnHeight;
        localPosition.z = Mathf.Clamp(localPosition.z, -halfArea.y, halfArea.y);

        return transform.TransformPoint(localPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(0f, spawnHeight, 0f),
            new Vector3(spawnArea.x, 0.1f, spawnArea.y));

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}
