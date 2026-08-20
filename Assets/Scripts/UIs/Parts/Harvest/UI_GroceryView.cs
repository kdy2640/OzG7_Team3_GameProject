using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GroceryView : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image iconImage;
    private GroceryType groceryType = GroceryType.Count;

    public void Initialize(GroceryType type)
    {
        groceryType = type;
        Refresh();
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

        iconImage.sprite = GroceryDataDB.GetData(groceryType).Icon;
    }
}
