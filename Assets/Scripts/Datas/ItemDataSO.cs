using UnityEngine;

public enum ItemType
{
    SpeedBoost,
    RangeBoost,
    DamageBoost,
    Count
}

[CreateAssetMenu(menuName = "Game/ItemDataSO")]
public sealed class ItemDataSO : ScriptableObject
{
    [SerializeField] private ItemType itemType = ItemType.Count;

    public ItemType ItemType => itemType;
}
