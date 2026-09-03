using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(GridChunkHandler))]
public sealed class ItemSpawner : MonoBehaviour
{
    private const int SpawnChanceDenominator = 9;

    [SerializeField] private ItemActor itemPrefab;
    [SerializeField, Min(0f)] private float cropClearance = 1.2f;

    private GridChunkHandler gridChunkHandler;

    private void Awake()
    {
        gridChunkHandler = GetComponent<GridChunkHandler>();
    }

    public void TrySpawn(
        Vector2Int chunkCoordinate,
        GridGeometry geometry,
        Transform parent)
    {
        if (Random.Range(0, SpawnChanceDenominator) != 0)
        {
            return;
        }

        List<Vector2> gapPositions = new(
            geometry.GetGapPositions(chunkCoordinate));

        while (gapPositions.Count > 0)
        {
            int index = Random.Range(0, gapPositions.Count);
            Vector2 localPosition = gapPositions[index];
            Vector3 worldPosition = transform.TransformPoint(
                new Vector3(localPosition.x, 1f, localPosition.y));

            if (gridChunkHandler.Registry.GetNearbyTransforms(
                    worldPosition,
                    cropClearance).Count == 0)
            {
                ItemType itemType = (ItemType)Random.Range(
                    0,
                    (int)ItemType.Count);
                ItemDataSO itemData = ItemDataDB.GetData(itemType);
                ItemActor item = Instantiate(
                    itemPrefab,
                    worldPosition,
                    transform.rotation,
                    parent);

                item.name = itemType.ToString();
                item.Init(itemData);
                return;
            }

            gapPositions.RemoveAt(index);
        }
    }
}
