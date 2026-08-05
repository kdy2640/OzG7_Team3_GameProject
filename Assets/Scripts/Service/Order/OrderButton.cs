using System;
using UnityEngine;

public class OrderButton : MonoBehaviour
{
    public event Action OnClicked;

    private DishAmount dishAmount;

    public void SetOrder(DishAmount order)
    {
        dishAmount = order;
    }

    public void OnClick()
    {
        if (dishAmount == null)
        {
            Debug.LogWarning("주문 정보가 없습니다.");
            return;
        }

        if (GameManager.Instance.StockManager.TryConsumeDish(dishAmount))
        {
            OnClicked?.Invoke();
        }
    }
}