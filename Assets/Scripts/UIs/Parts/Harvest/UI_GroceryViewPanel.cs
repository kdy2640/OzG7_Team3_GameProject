using System.Collections.Generic;
using UnityEngine;

public class UI_GroceryViewPanel : MonoBehaviour
{
    [SerializeField] private List<GroceryType> groceryTypes = new();
    [SerializeField] private UI_GroceryView groceryViewPrefab;
    [SerializeField] private Transform viewContainer;

    private readonly List<UI_GroceryView> groceryViews = new();
    private StockManager stockManager;
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
        stockManager?.SubscribeStockDataChange(Refresh);
        isReady = true;
        RefreshGroceryViews();
    }

    private void OnDestroy()
    {
        stockManager?.UnsubscribeStockDataChange(Refresh);
    }

    public void Refresh()
    {
        for (int i = 0; i < groceryViews.Count; i++)
        {
            if (groceryViews[i].gameObject.activeSelf)
            {
                groceryViews[i].Refresh();
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
            }
        }
    }
}
