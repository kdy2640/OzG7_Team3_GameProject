#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StockManager
{
    private const string DebugPanelName = "Game / Stock";

    private DebugUI.Panel debugPanel;

    internal void RegisterDebugUI()
    {
        debugPanel = DebugManager.instance.GetPanel(
            DebugPanelName,
            true,
            0,
            true);
        RebuildDebugUI();
    }

    internal void UnregisterDebugUI()
    {
        if (debugPanel == null)
            return;

        DebugManager.instance.RemovePanel(debugPanel);
        debugPanel = null;
    }

    private void RebuildDebugUI()
    {
        debugPanel.children.Clear();
        debugPanel.children.Add(new DebugUI.MessageBox
        {
            displayName = "Raw stock data. Changes can be saved on scene change or application quit.",
            style = DebugUI.MessageBox.Style.Warning
        });
        debugPanel.children.Add(new DebugUI.IntField
        {
            displayName = "Currency",
            getter = () => stockData.currency,
            setter = value =>
            {
                stockData.currency = value;
                NotifyStockDataChanged();
            }
        });

        DebugUI.Foldout groceries = new()
        {
            displayName = "Groceries",
            opened = false
        };

        for (int i = 0; i < stockData.groceries.Count; i++)
        {
            int index = i;
            GroceryAmount entry = stockData.groceries[index];
            DebugUI.Foldout row = new()
            {
                displayName = $"[{index}]",
                opened = false
            };

            if (entry == null)
            {
                row.children.Add(new DebugUI.MessageBox
                {
                    displayName = "null entry",
                    style = DebugUI.MessageBox.Style.Error
                });
            }
            else
            {
                DebugUI.EnumField groceryType = null;
                groceryType = new DebugUI.EnumField
                {
                    displayName = "Grocery",
                    autoEnum = typeof(GroceryType),
                    getter = () => (int)entry.grocery,
                    setter = value =>
                    {
                        entry.grocery = (GroceryType)value;
                        NotifyStockDataChanged();
                    },
                    getIndex = () => Array.IndexOf(
                        groceryType.enumValues,
                        (int)entry.grocery),
                    setIndex = _ => { }
                };
                row.children.Add(groceryType);
                row.children.Add(new DebugUI.IntField
                {
                    displayName = "Amount",
                    getter = () => entry.amount,
                    setter = value =>
                    {
                        entry.amount = value;
                        NotifyStockDataChanged();
                    }
                });
            }

            row.children.Add(new DebugUI.Button
            {
                displayName = "Remove",
                action = () =>
                {
                    stockData.groceries.RemoveAt(index);
                    NotifyStockDataChanged();
                    RebuildDebugUI();
                }
            });
            groceries.children.Add(row);
        }

        groceries.children.Add(new DebugUI.Button
        {
            displayName = "Add Entry",
            action = () =>
            {
                stockData.groceries.Add(new GroceryAmount());
                NotifyStockDataChanged();
                RebuildDebugUI();
            }
        });
        debugPanel.children.Add(groceries);

        DebugUI.Foldout dishes = new()
        {
            displayName = "Dishes",
            opened = false
        };

        for (int i = 0; i < stockData.dishes.Count; i++)
        {
            int index = i;
            DishAmount entry = stockData.dishes[index];
            DebugUI.Foldout row = new()
            {
                displayName = $"[{index}]",
                opened = false
            };

            if (entry == null)
            {
                row.children.Add(new DebugUI.MessageBox
                {
                    displayName = "null entry",
                    style = DebugUI.MessageBox.Style.Error
                });
            }
            else
            {
                DebugUI.EnumField dishType = null;
                dishType = new DebugUI.EnumField
                {
                    displayName = "Dish",
                    autoEnum = typeof(DishType),
                    getter = () => (int)entry.dish,
                    setter = value =>
                    {
                        entry.dish = (DishType)value;
                        NotifyStockDataChanged();
                    },
                    getIndex = () => Array.IndexOf(
                        dishType.enumValues,
                        (int)entry.dish),
                    setIndex = _ => { }
                };
                row.children.Add(dishType);
                row.children.Add(new DebugUI.IntField
                {
                    displayName = "Amount",
                    getter = () => entry.amount,
                    setter = value =>
                    {
                        entry.amount = value;
                        NotifyStockDataChanged();
                    }
                });
            }

            row.children.Add(new DebugUI.Button
            {
                displayName = "Remove",
                action = () =>
                {
                    stockData.dishes.RemoveAt(index);
                    NotifyStockDataChanged();
                    RebuildDebugUI();
                }
            });
            dishes.children.Add(row);
        }

        dishes.children.Add(new DebugUI.Button
        {
            displayName = "Add Entry",
            action = () =>
            {
                stockData.dishes.Add(new DishAmount());
                NotifyStockDataChanged();
                RebuildDebugUI();
            }
        });
        debugPanel.children.Add(dishes);
        DebugManager.instance.ReDrawOnScreenDebug();
    }
}
#endif
