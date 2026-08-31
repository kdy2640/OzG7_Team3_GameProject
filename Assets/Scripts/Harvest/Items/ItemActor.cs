using UnityEngine;

[DisallowMultipleComponent]
public sealed class ItemActor : MonoBehaviour
{
    private ItemDataSO itemData;

    public ItemDataSO ItemData => itemData;

    public void Init(ItemType itemType)
    {
        itemData = ItemDataDB.GetData(itemType);
    }
}
