using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class HarvestSpawner : MonoBehaviour
{
    [SerializeField] private HarvestActor cropPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private StageType stageType = StageType.Stage_1;
    [SerializeField, Min(0)] private int spawnCount = 60;
    [SerializeField] private Vector2 spawnArea = new(9f, 9f);
    [SerializeField] private float spawnHeight;
    [SerializeField] private List<HarvestActor> spawnedCrops = new();

    private Transform cropContainer;

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

        foreach (HarvestActor crop in spawnedCrops)
        {
            if (crop != null)
                Destroy(crop.gameObject);
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

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 worldPosition = GetRandomPosition();
            Quaternion rotation =
                transform.rotation * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            HarvestType randomType =
                stageData.HarvestList[Random.Range(0, stageData.HarvestList.Count)];

            HarvestActor crop = Instantiate(
                cropPrefab,
                worldPosition,
                rotation,
                cropContainer);
            crop.name = "Crop";
            crop.Init(randomType, player, this);
            spawnedCrops.Add(crop);
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
