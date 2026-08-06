using System.Collections.Generic;
using UnityEngine;

public class DishStorageUI : MonoBehaviour
{
    [SerializeField] private DishSlot[] slots;

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        IReadOnlyList<DishAmount> dishes = GameManager.Instance.StockManager.StockData.Dishes;

        for (int i = 0; i < slots.Length; i++)
        {
            if(i<dishes.Count)
            {
                slots[i].SetDish(dishes[i]);
            }
            else
            {
                slots[i].Clear();
            }
        }
    }
}
