using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GridChunkHandler))]
public sealed class HarvestSpawner : MonoBehaviour
{
    [SerializeField] private HarvestActor cropPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private StageType stageType = StageType.Stage_1;
    [SerializeField] private float spawnHeight;
    [SerializeField] private List<HarvestActor> spawnedCrops = new();

    private readonly List<HarvestType> staticTypes = new();
    private readonly List<HarvestType> movableTypes = new();

    private GridChunkHandler gridChunkHandler;

    public bool HasStaticTypes => staticTypes.Count > 0;
    public bool HasMovableTypes => movableTypes.Count > 0;

    private void Awake()
    {
        gridChunkHandler = GetComponent<GridChunkHandler>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        StageDataSO stageData = StageDataDB.GetData(stageType);
        staticTypes.Clear();
        movableTypes.Clear();

        foreach (HarvestType harvestType in stageData.HarvestList)
        {
            HarvestDataSO harvestData = HarvestDataDB.GetData(harvestType);

            if (harvestData.IsMove)
            {
                movableTypes.Add(harvestType);
            }
            else
            {
                staticTypes.Add(harvestType);
            }
        }

        ClearSpawnedCrops();

        gridChunkHandler.Streamer.BeginLoading(player, this);
    }

    public void SpawnStaticCrop(
        Vector2 localPosition,
        Transform parent)
    {
        HarvestType type = staticTypes[Random.Range(0, staticTypes.Count)];

        CreateCrop(type, localPosition, parent);
    }

    public void SpawnMovableCrop(
        Vector2 localPosition,
        Transform parent)
    {
        HarvestType type = movableTypes[Random.Range(0, movableTypes.Count)];

        CreateCrop(type, localPosition, parent);
    }

    private void ClearSpawnedCrops()
    {
        foreach (HarvestActor crop in spawnedCrops)
        {
            gridChunkHandler.Registry.Unregister(crop.transform);
        }

        spawnedCrops.Clear();
        gridChunkHandler.Streamer.Reset();
    }

    private void CreateCrop(
        HarvestType type,
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
        crop.Init(type, player, gridChunkHandler);
        spawnedCrops.Add(crop);
    }
}
