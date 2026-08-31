using UnityEngine;

[DisallowMultipleComponent]
public sealed class ItemSpawner : MonoBehaviour
{
    private const int SpawnChanceDenominator = 9;

    [SerializeField] private ItemActor itemPrefab;

    public void TrySpawn(
        Vector2Int chunkCoordinate,
        GridGeometry geometry,
        Transform parent)
    {
        if (Random.Range(0, SpawnChanceDenominator) != 0)
        {
            return;
        }

        ItemType itemType = (ItemType)Random.Range(0, (int)ItemType.Count);
        ItemDataSO itemData = ItemDataDB.GetData(itemType);
        Vector2 localPosition = geometry.GetRandomPositionInChunk(
            chunkCoordinate);
        Vector3 worldPosition = transform.TransformPoint(
            new Vector3(localPosition.x, 1f, localPosition.y));
        ItemActor item = Instantiate(
            itemPrefab,
            worldPosition,
            transform.rotation,
            parent);

        item.name = itemType.ToString();
        GameObject solidModel = Instantiate(itemData.SolidModel, item.transform);
        solidModel.transform.SetLocalPositionAndRotation(
            Vector3.zero,
            Quaternion.identity);
        item.Init(itemData);
    }
}
