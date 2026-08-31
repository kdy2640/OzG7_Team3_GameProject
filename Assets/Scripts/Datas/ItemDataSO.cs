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
    [SerializeField] private GameObject solidModel;
    [SerializeField, Min(0f)] private float effectAmount = 0.3f;

    public ItemType ItemType => itemType;
    public GameObject SolidModel => solidModel;
    public float EffectAmount => effectAmount;
}
