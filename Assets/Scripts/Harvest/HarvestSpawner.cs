using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GridChunkHandler))]
public sealed class HarvestSpawner : MonoBehaviour
{
    private const float StageBoundaryEpsilon = 0.01f;

    [SerializeField] private HarvestActor cropPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnHeight;
    [SerializeField] private List<HarvestActor> spawnedCrops = new();

    private GridChunkHandler gridChunkHandler;
    private HarvestEmployeeResolver employeeResolver;
    private HarvestManager harvestManager;
    private bool hasSpawnedPig;
    private HarvestActor spawnedPig;

    private void Awake()
    {
        gridChunkHandler = GetComponent<GridChunkHandler>();
        employeeResolver = player.GetComponent<HarvestEmployeeResolver>();
    }

    private void Start()
    {
        Initialize();
        harvestManager = GameManager.Instance.Harvest;
        harvestManager.Events.Subscribe(
            HarvestEventType.LoopStarted,
            SpawnPig);
    }

    private void OnDestroy()
    {
        harvestManager.Events.Unsubscribe(
            HarvestEventType.LoopStarted,
            SpawnPig);
    }

    public void Initialize()
    {
        hasSpawnedPig = false;
        ClearSpawnedCrops();

        gridChunkHandler.Streamer.BeginLoading(player, this);
    }

    private void SpawnPig()
    {
        if (hasSpawnedPig)
            return;

        hasSpawnedPig = true;

        Rect area = gridChunkHandler.Geometry.Area;
        float stageMinZ = StageDataDB.GetData(StageType.Stage_2).ZStart;
        float stageMaxZ = StageDataDB.GetData(StageType.Stage_3).ZStart
            - StageBoundaryEpsilon;
        Vector2 localPosition = new(
            Random.Range(area.xMin, area.xMax),
            Random.Range(stageMinZ, stageMaxZ));

        spawnedPig = CreateCrop(
            HarvestType.Pig,
            StageType.Stage_2,
            localPosition,
            transform);
    }

    public void SpawnCrop(
        HarvestType type,
        StageType stageType,
        Vector2 localPosition,
        Transform parent)
    {
        CreateCrop(type, stageType, localPosition, parent);
    }

    private void ClearSpawnedCrops()
    {
        foreach (HarvestActor crop in spawnedCrops)
        {
            gridChunkHandler.Registry.Unregister(crop.transform);
        }

        if (spawnedPig != null)
        {
            Destroy(spawnedPig.gameObject);
            spawnedPig = null;
        }

        spawnedCrops.Clear();
        gridChunkHandler.Streamer.Reset();
    }

    private HarvestActor CreateCrop(
        HarvestType type,
        StageType stageType,
        Vector2 localPosition,
        Transform parent)
    {
        Vector3 worldPosition = transform.TransformPoint(
            new Vector3(localPosition.x, spawnHeight, localPosition.y));
        HarvestActor crop = Instantiate(
            cropPrefab,
            worldPosition,
            transform.rotation,
            parent);

        crop.name = "Crop";
        crop.Init(
            type,
            stageType,
            player,
            gridChunkHandler,
            employeeResolver);
        spawnedCrops.Add(crop);
        return crop;
    }
}
