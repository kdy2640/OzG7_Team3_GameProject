using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CookSlot : MonoBehaviour
{
    public event Action OnClicked;

    [SerializeField] private DishType dishType;
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text amountText;
    
    private void OnEnable()
    {
        GameManager.Instance.StockManager.SubscribeStockDataChange(UpdateUI);
        UpdateUI();
    }

    public void UpdateUI()
    {
        DishDataSO data = DishDataDB.GetData(dishType);

        if (data == null) return;

        dishName.text = data.DisplayName;
        stateText.text = "No\nGrocery";

        stateText.enabled = false;

        amountText.text = $"x {GetDishAmount()}";

        bool canCook = GameManager.Instance.CookingManager.CanCook(dishType);

        
        if ( !canCook )
        {
            dishName.enabled = false;
            stateText.enabled = true;
        }
    }

    private int GetDishAmount()
    {
        IReadOnlyList<DishAmount> dishes =
            GameManager.Instance.StockManager.StockData.Dishes;

        foreach (DishAmount dish in dishes)
        {
            if (dish.dish == dishType)
            {
                return dish.amount;
            }
        }

        return 0;
    }

    public void OnClick()
    {
        if (GameManager.Instance.CookingManager.TryCook(dishType))
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