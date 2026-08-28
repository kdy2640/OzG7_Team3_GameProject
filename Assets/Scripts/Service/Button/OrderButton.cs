using System;
using System.Linq;
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
    [SerializeField] private Image autoServeImg;
    [SerializeField] private Image cookingImg;
    [SerializeField] private Image foodReadyImg;
    [SerializeField] private Image foodIcon;

    private CookingList cookingList;

    public Image AutoServeImg => autoServeImg;
    public CustomerStateManager Customer => customer;

    public bool IsAutoServing = false;
    public bool IsCooking = false;

    private void OnEnable()
    {
        customer = GetComponentInParent<CustomerStateManager>();
        serverList = FindFirstObjectByType<ServerList>();
        cookingList = FindFirstObjectByType<CookingList>();
        
        autoServeImg.gameObject.SetActive(false);
        cookingList.cookingListChanged += UpdateCookingUI;
        UpdateCookingUI();

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
        foodIcon.sprite = DishDataDB.GetData(order.dish).Icon;
    }

    private void UpdateCookingUI()
    {
        if (dishAmount == null)
            return;

        if (cookingList != null)
        {
            cookingImg.gameObject.SetActive(
            cookingList.List.Contains(dishAmount.dish)
            );
        }
        
        foodReadyImg.gameObject.SetActive(
            GameManager.Instance.StockManager.CanConsumeDish(dishAmount)
            );
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
        }
    }

    

    private void OnDisable()
    {
        cookingList.cookingListChanged -= UpdateCookingUI;
    }

    private void OnDestroy()
    {
        cookingList.cookingListChanged -= UpdateCookingUI;
    }
}