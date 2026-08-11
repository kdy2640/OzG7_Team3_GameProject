using System;
using TMPro;
using UnityEngine;

public class OrderButton : MonoBehaviour
{
    [SerializeField] ServerList serverList;
    private CustomerStateManager customer;
    public event Action OnClicked;
    private DishAmount dishAmount;

    //[SerializeField] private Image dishIcon;
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text amountText;

    private void OnEnable()
    {
        customer = GetComponentInParent<CustomerStateManager>();
    }

    public void SetOrder(DishAmount order)
    {
        dishAmount = order;
        DishDataSO data = DishDataDB.GetData(order.dish);
        if (data == null)
        {
            Debug.LogWarning($"DishDataSO를 찾을 수 없습니다. {order.dish}");
            return;
        }

        //dishIcon.sprite = data.Icon;
        dishName.text = data.DisplayName;
        amountText.text = order.amount.ToString();
    }

    public void OnClick()
    {
        if (dishAmount == null)
        {
            Debug.LogWarning("주문 정보가 없습니다.");
            return;
        }

        if(!serverList.TryAllocServe(dishAmount.dish, customer))
        {
            return;
        }

        if (GameManager.Instance.StockManager.TryConsumeDish(dishAmount))
        {
            OnClicked?.Invoke();
        }
    }
}