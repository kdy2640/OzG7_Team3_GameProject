using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GroceryView : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;

    private GroceryType groceryType = GroceryType.Count;

    public void Initialize(GroceryType type)
    {
        groceryType = type;
    }

    public void Refresh()
    {
        if (groceryType == GroceryType.Count || amountText == null)
            return;

        IReadOnlyList<GroceryAmount> groceries =
            GameManager.Instance?.StockManager?.StockData?.Groceries;

        if (groceries == null)
            return;

        long amount = 0;

        for (int i = 0; i < groceries.Count; i++)
        {
            GroceryAmount grocery = groceries[i];

            if (grocery != null && grocery.grocery == groceryType)
                amount += grocery.amount;
        }

        amountText.text = amount.ToString();
    }
}
