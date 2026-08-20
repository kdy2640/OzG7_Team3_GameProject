using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_NewDishList : MonoBehaviour
{
    private const int SlotCount = 5;

    private readonly GameObject[] slots = new GameObject[SlotCount];
    private readonly Image[] dishImages = new Image[SlotCount];
    private bool areSlotsCached;

    public void Refresh()
    {
        CacheSlots();
        ClearSlots();

        MarketManager marketManager = GameManager.Instance?.Market;

        if (marketManager == null)
        {
            Debug.LogError($"[{nameof(UI_NewDishList)}] MarketManager를 찾을 수 없습니다.", this);
            return;
        }

        int currentLevel = marketManager.MarketData.CurrentLevel;
        List<DishDataSO> dishes = new();

        for (int i = 0; i < (int)DishType.Count; i++)
        {
            if (DishDataDB.TryGetData((DishType)i, out DishDataSO dishData)
                && dishData != null
                && dishData.Tier == currentLevel)
            {
                dishes.Add(dishData);
            }
        }

        int count = Mathf.Min(dishes.Count, SlotCount);
        int startIndex = (SlotCount - count) / 2;

        for (int i = 0; i < count; i++)
        {
            int slotIndex = startIndex + i;
            Sprite icon = dishes[i].Icon;

            slots[slotIndex]?.SetActive(true);

            if (dishImages[slotIndex] != null)
            {
                dishImages[slotIndex].sprite = icon;
                dishImages[slotIndex].enabled = icon != null;
            }
        }
    }

    private void CacheSlots()
    {
        if (areSlotsCached)
            return;

        areSlotsCached = true;

        for (int i = 0; i < SlotCount; i++)
        {
            Transform slot = transform.Find($"UnlockedMenu{i + 1:00}");
            slots[i] = slot?.gameObject;
            dishImages[i] = slot?.Find("MenuImage")?.GetComponent<Image>();
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            slots[i]?.SetActive(false);

            if (dishImages[i] != null)
            {
                dishImages[i].sprite = null;
                dishImages[i].enabled = false;
            }
        }
    }
}
