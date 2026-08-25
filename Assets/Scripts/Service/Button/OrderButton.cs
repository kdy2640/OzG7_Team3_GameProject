using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderButton : MonoBehaviour
{
    public event Action OnClicked;
    private DishAmount dishAmount;

    [SerializeField] ServerList serverList;
    private CustomerStateManager customer;
    //[SerializeField] private Image dishIcon;
    [SerializeField] private TMP_Text dishName;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image waitingForFoodImg;
    [SerializeField] private Image autoServeImg;

    public bool IsAutoServing = false;


    private void OnEnable()
    {
        customer = GetComponentInParent<CustomerStateManager>();
        serverList = FindFirstObjectByType<ServerList>();
        
        waitingForFoodImg.gameObject.SetActive(false);
        autoServeImg.gameObject.SetActive(false);
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
            if(IsAutoServing)
            {
                waitingForFoodImg.gameObject.SetActive(false);
                autoServeImg.gameObject.SetActive(true);
            }
            else
            {
                autoServeImg.gameObject.SetActive(false);
                waitingForFoodImg.gameObject.SetActive(true);
            }
        }
    }
}