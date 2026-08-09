using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CookSlot : MonoBehaviour
{
    public event Action OnClicked;

    
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text amountText;

    [SerializeField] CookingQueue cookingQueue;

    private DishType dish;

    public void Initialize(DishType dish)
    {
        this.dish = dish;
        UpdateUI();
        if(cookingQueue == null)
        {
            cookingQueue = transform.parent.parent.GetComponentInChildren<CookingQueue>();
        }
    }
    
    private void OnEnable()
    {
        GameManager.Instance.StockManager.SubscribeStockDataChange(UpdateUI);
        UpdateUI();
    }

    public void UpdateUI()
    {
        DishDataSO data = DishDataDB.GetData(dish);

        if (data == null) return;

        dishName.text = data.DisplayName;
        stateText.text = "No\nGrocery";

        stateText.enabled = false;

        amountText.text = $"x {GetDishAmount()}";

        bool canCook = GameManager.Instance.CookingManager.CanCook(dish);

        
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
        if (!cookingQueue.CanRequestCook()) return;

        if(GameManager.Instance.CookingManager.TryCook(dish))
        {
            cookingQueue.RequestCook(dish);

            UpdateUI();
            OnClicked?.Invoke();
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.StockManager.UnsubscribeStockDataChange(UpdateUI);
    }
}