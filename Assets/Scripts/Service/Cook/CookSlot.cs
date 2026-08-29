using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookSlot : MonoBehaviour
{
    public event Action OnClicked;

    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text countTextShadow;
    [SerializeField] private TMP_Text canCookText;
    [SerializeField] private Image dishIcon;
    [SerializeField] private GameObject servingCountBackground;
    [SerializeField] private GameObject lockBackground;

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

        int dishAmount = GetDishAmount();
        string countLabel = $"x {dishAmount}";
        int cookableAmount = GameManager.Instance.CookingManager.CalculateCookableAmount(dish);

        countText.text = countLabel;
        countTextShadow.text = countLabel;
        canCookText.text = $"x {cookableAmount}";
        dishIcon.sprite = data.Icon;
        servingCountBackground.SetActive(dishAmount > 0);
        lockBackground.SetActive(cookableAmount <= 0);
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
