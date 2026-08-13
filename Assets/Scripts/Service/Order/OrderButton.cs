using System;
using TMPro;
using UnityEngine;

public class OrderButton : MonoBehaviour
{
    public event Action OnClicked;
    public event Action NoFreeServer;
    private DishAmount dishAmount;


    [SerializeField] ServerList serverList;
    private CustomerStateManager customer;
    //[SerializeField] private Image dishIcon;
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text amountText;

    private void OnEnable()
    {
        customer = GetComponentInParent<CustomerStateManager>();
        serverList = FindFirstObjectByType<ServerList>();
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
        if (!GameManager.Instance.StockManager.CanConsumeDish(dishAmount))
        {
            return;
        }


        if (!serverList.TryAllocServe(dishAmount.dish, customer))
        {
            NoFreeServer?.Invoke();
            return;
        }

        if (dishAmount == null)
        {
            Debug.LogWarning("주문 정보가 없습니다.");
            return;
        }

        if (GameManager.Instance.StockManager.TryConsumeDish(dishAmount))
        {
            OnClicked?.Invoke();
            Debug.Log("주문 수락 성공");
        }
    }
}