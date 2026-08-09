
using System.Collections.Generic;
using UnityEngine;

public class KitchenSlotContainer : MonoBehaviour
{
    [SerializeField] private int slotCount = 2;

    [SerializeField] KitchenSlot kitchenSlotPrefab;

    private List<KitchenSlot> slots = new List<KitchenSlot>();
    [SerializeField] private CookingQueue cookingQueue;

    private void OnEnable()
    {
        CreateKitchenSlot();
    }

    private void CreateKitchenSlot()
    {
        for (int i = 0; i < slotCount; i++)
        {
            KitchenSlot slot = Instantiate(kitchenSlotPrefab, transform);
            slot.Initialize();
            slots.Add (slot);
        }
    }

    private void Update()
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
                continue;

            if (cookingQueue.TryGetNextDish(out DishType dish))
            {
                slot.StartCooking(dish);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();
    }
}
