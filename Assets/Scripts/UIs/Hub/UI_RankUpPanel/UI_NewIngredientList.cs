using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_NewIngredientList : MonoBehaviour
{
    private const int SlotCount = 5;

    private readonly GameObject[] slots = new GameObject[SlotCount];
    private readonly Image[] groceryImages = new Image[SlotCount];
    private bool areSlotsCached;

    public void Refresh()
    {
        CacheSlots();
        ClearSlots();

        MarketManager marketManager = GameManager.Instance?.Market;

        if (marketManager == null)
        {
            Debug.LogError($"[{nameof(UI_NewIngredientList)}] MarketManager를 찾을 수 없습니다.", this);
            return;
        }

        int currentLevel = marketManager.MarketData.CurrentLevel;
        List<GroceryDataSO> groceries = new();

        for (int i = 0; i < (int)GroceryType.Count; i++)
        {
            if (GroceryDataDB.TryGetData((GroceryType)i, out GroceryDataSO groceryData)
                && groceryData != null
                && groceryData.Tier == currentLevel)
            {
                groceries.Add(groceryData);
            }
        }

        int count = Mathf.Min(groceries.Count, SlotCount);
        int startIndex = (SlotCount - count) / 2;

        for (int i = 0; i < count; i++)
        {
            int slotIndex = startIndex + i;
            Sprite icon = groceries[i].Icon;

            slots[slotIndex]?.SetActive(true);

            if (groceryImages[slotIndex] != null)
            {
                groceryImages[slotIndex].sprite = icon;
                groceryImages[slotIndex].enabled = icon != null;
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
            Transform slot = transform.Find($"UnlockedIngredinet{i + 1:00}");
            slots[i] = slot?.gameObject;
            groceryImages[i] = slot?.Find("MenuImage")?.GetComponent<Image>();
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            slots[i]?.SetActive(false);

            if (groceryImages[i] != null)
            {
                groceryImages[i].sprite = null;
                groceryImages[i].enabled = false;
            }
        }
    }
}
