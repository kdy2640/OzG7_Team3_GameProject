using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(ItemPresenter))]
public sealed class ItemActor : MonoBehaviour
{
    [SerializeField] private ItemPresenter presenter;

    private ItemDataSO itemData;
    private bool isCollected;

    public ItemDataSO ItemData => itemData;

    public void Init(ItemDataSO data)
    {
        itemData = data;
        presenter.Init(itemData.SolidModel);
    }

    private void OnTriggerEnter(Collider other)
    {
        TractorController tractor =
            other.GetComponentInParent<TractorController>();

        if (tractor == null || isCollected)
        {
            return;
        }

        isCollected = true;

        switch (itemData.ItemType)
        {
            case ItemType.SpeedBoost:
                tractor.ApplySpeedBoost(itemData.EffectAmount);
                break;
            case ItemType.RangeBoost:
                tractor.ApplyRangeBoost(itemData.EffectAmount);
                break;
            case ItemType.DamageBoost:
                tractor.ApplyDamageBoost(itemData.EffectAmount);
                break;
        }

        Destroy(gameObject);
    }
}
