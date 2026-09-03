using System.Collections.Generic;
using UnityEngine;

public class UI_GroceryViewPanel : MonoBehaviour
{
    [SerializeField] private List<GroceryType> groceryTypes = new();
    [SerializeField] private UI_GroceryView groceryViewPrefab;
    [SerializeField] private Transform viewContainer;

    private readonly List<UI_GroceryView> groceryViews = new();
    private readonly long[] displayAmounts =
        new long[(int)GroceryType.Count];
    private StockManager stockManager;
    private bool isDelayRefresh;
    private bool isReady;

    private void Start()
    {
        if (groceryViewPrefab == null || viewContainer == null)
        {
            Debug.LogError(
                $"[{nameof(UI_GroceryViewPanel)}] GroceryView prefab and view container are required.",
                this);
            return;
        }

        stockManager = GameManager.Instance?.StockManager;
        stockManager?.SubscribeStockDataChange(OnStockDataChanged);
        isReady = true;
        RefreshGroceryViews();
    }

    private void OnDestroy()
    {
        stockManager?.UnsubscribeStockDataChange(OnStockDataChanged);
    }

    public void SetDelayRefresh(bool value)
    {
        isDelayRefresh = value;
    }

    public void Refresh()
    {
        for (int i = 0; i < groceryViews.Count; i++)
        {
            if (groceryViews[i].gameObject.activeSelf)
            {
                RefreshView(i);
            }
        }
    }

    public void RefreshOneUI(
        GroceryAmount reward,
        bool forceRealValue)
    {
        int groceryIndex = (int)reward.grocery;
        long displayAmount = forceRealValue
            ? GetStockAmount(reward.grocery)
            : displayAmounts[groceryIndex] + reward.amount;

        displayAmounts[groceryIndex] = displayAmount;

        for (int i = 0; i < groceryTypes.Count; i++)
        {
            if (groceryTypes[i] == reward.grocery)
            {
                groceryViews[i].SetAmount(displayAmount);
                groceryViews[i].PlayGain();
                return;
            }
        }
    }

    public void Initialize(IReadOnlyList<GroceryType> types)
    {
        groceryTypes.Clear();

        if (types != null)
        {
            for (int i = 0; i < types.Count; i++)
            {
                groceryTypes.Add(types[i]);
            }
        }

        if (isReady)
        {
            RefreshGroceryViews();
        }
    }

    private void RefreshGroceryViews()
    {
        for (int i = groceryViews.Count; i < groceryTypes.Count; i++)
        {
            UI_GroceryView groceryView = Instantiate(
                groceryViewPrefab,
                viewContainer);
            groceryViews.Add(groceryView);
        }

        for (int i = 0; i < groceryViews.Count; i++)
        {
            UI_GroceryView groceryView = groceryViews[i];
            bool isVisible = i < groceryTypes.Count;
            groceryView.gameObject.SetActive(isVisible);

            if (isVisible)
            {
                groceryView.Initialize(groceryTypes[i]);
                RefreshView(i);
            }
        }
    }

    private void RefreshView(int index)
    {
        GroceryType groceryType = groceryTypes[index];
        long amount = GetStockAmount(groceryType);

        displayAmounts[(int)groceryType] = amount;
        groceryViews[index].SetAmount(amount);
    }

    private long GetStockAmount(GroceryType groceryType)
    {
        IReadOnlyList<GroceryAmount> groceries = stockManager.StockData.Groceries;
        long amount = 0;

        for (int i = 0; i < groceries.Count; i++)
        {
            GroceryAmount grocery = groceries[i];

            if (grocery != null && grocery.grocery == groceryType)
            {
                amount += grocery.amount;
            }
        }

        return amount;
    }

    private void OnStockDataChanged()
    {
        if (isDelayRefresh)
        {
            return;
        }

        Refresh();
    }
}
