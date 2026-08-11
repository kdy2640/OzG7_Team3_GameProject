using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookSlot : MonoBehaviour
{
    public event Action OnClicked;

    
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text canCookText;
    [SerializeField] private Image dishIcon;

    private DishType dish;
    private KitchenSlotHandler kitchenSlotHandler;
    private bool isInitialized;

    public void Initialize(DishType dish, KitchenSlotHandler kitchenSlotHandler)
    {
        this.dish = dish;
        this.kitchenSlotHandler = kitchenSlotHandler;
        isInitialized = true;
        UpdateUI();
    }
    
    private void OnEnable()
    {
        GameManager.Instance.StockManager.SubscribeStockDataChange(UpdateUI);

        if (isInitialized)
            UpdateUI();
    }

    public void UpdateUI()
    {
        if (!isInitialized)
            return;

        DishDataSO data = DishDataDB.GetData(dish);

        if (data == null) return;

        dishName.text = data.DisplayName;
        countText.text = $"x {GetDishAmount()}";
        canCookText.text = $"x {GameManager.Instance.CookingManager.CalculateCookableAmount(dish)}";
        dishIcon.sprite = data.Icon;
    }

    private int GetDishAmount()
    {
        IReadOnlyList<DishAmount> dishes =
            GameManager.Instance.StockManager.StockData.Dishes;

        foreach (DishAmount dishAmount in dishes)
        {
            if (dishAmount.dish == this.dish)
            {
                return dishAmount.amount;
            }
        }

        return 0;
    }

    public void OnClick()
    {
        if (kitchenSlotHandler == null)
            return;

        if (kitchenSlotHandler.TryRequestCook(dish))
        {
            UpdateUI();
            OnClicked?.Invoke();
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.StockManager.UnsubscribeStockDataChange(UpdateUI);
    }
}
