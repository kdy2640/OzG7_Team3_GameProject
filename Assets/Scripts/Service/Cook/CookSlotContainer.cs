using System.Collections.Generic;
using UnityEngine;

public class CookSlotContainer : MonoBehaviour
{
    [SerializeField] private CookSlot CookSlotPrefab;

    private bool initialized;

    private void OnEnable()
    {
        GameManager.Instance.Market.SubscribeMarketDataChanged(RefreshSlots);
        if (initialized) return;
        CreateCookSlot();
        initialized = true;
    }

    private void CreateCookSlot()
    {
        foreach (DishType dish in GameManager.Instance.Market.Data.SelectedDishes)
        {
            CookSlot slot = Instantiate(CookSlotPrefab, transform);
            slot.Initialize(dish);
        }

    }

    private void ClearSlots()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void RefreshSlots()
    {
        ClearSlots();
        CreateCookSlot();
    }
}
