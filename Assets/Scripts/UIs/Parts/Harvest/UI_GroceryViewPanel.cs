using System.Collections.Generic;
using UnityEngine;

public class UI_GroceryViewPanel : MonoBehaviour
{
    [SerializeField] private List<GroceryType> groceryTypes = new();
    [SerializeField] private UI_GroceryView groceryViewPrefab;
    [SerializeField] private Transform viewContainer;

    private readonly List<UI_GroceryView> groceryViews = new();
    private StockManager stockManager;

    private void Start()
    {
        if (groceryViewPrefab == null || viewContainer == null)
        {
            Debug.LogError(
                $"[{nameof(UI_GroceryViewPanel)}] GroceryView prefab and view container are required.",
                this);
            return;
        }

        CreateGroceryViews();

        stockManager = GameManager.Instance?.StockManager;
        stockManager?.SubscribeStockDataChange(Refresh);
        Refresh();
    }

    private void OnDestroy()
    {
        stockManager?.UnsubscribeStockDataChange(Refresh);
    }

    public void Refresh()
    {
        for (int i = 0; i < groceryViews.Count; i++)
            groceryViews[i].Refresh();
    }

    private void CreateGroceryViews()
    {
        if (groceryTypes == null)
            return;

        for (int i = 0; i < groceryTypes.Count; i++)
        {
            UI_GroceryView groceryView = Instantiate(groceryViewPrefab, viewContainer);
            groceryView.Initialize(groceryTypes[i]);
            groceryViews.Add(groceryView);
        }
    }
}
