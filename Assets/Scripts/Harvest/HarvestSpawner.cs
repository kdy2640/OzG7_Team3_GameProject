using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GridChunkHandler))]
public sealed class HarvestSpawner : MonoBehaviour
{
    [SerializeField] private HarvestActor cropPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnHeight;
    [SerializeField] private List<HarvestActor> spawnedCrops = new();

    private GridChunkHandler gridChunkHandler;

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
        ClearSpawnedCrops();

        gridChunkHandler.Streamer.BeginLoading(player, this);
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

        spawnedCrops.Clear();
        gridChunkHandler.Streamer.Reset();
    }

    private void CreateCrop(
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
        crop.Init(type, stageType, player, gridChunkHandler);
        spawnedCrops.Add(crop);
    }
}
