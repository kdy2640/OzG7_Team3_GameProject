using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class MarketManager
{
    private const string DebugPanelName = "Game / Market";

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
            displayName = "Raw market data. Level changes refresh dependent level and upgrade runtime data.",
            style = DebugUI.MessageBox.Style.Warning
        });

        DebugUI.Foldout data = new()
        {
            displayName = "Market Data",
            opened = true
        };
        data.children.Add(new DebugUI.IntField
        {
            displayName = "Current Business Day",
            getter = () => marketData.currentBusinessDay,
            setter = value =>
            {
                marketData.currentBusinessDay = value;
                NotifyMarketDataChanged();
            }
        });

        DebugUI.EnumField phase = null;
        phase = new DebugUI.EnumField
        {
            displayName = "Current Phase",
            autoEnum = typeof(MarketPhase),
            getter = () => (int)marketData.currentPhase,
            setter = value =>
            {
                marketData.currentPhase = (MarketPhase)value;
                NotifyMarketDataChanged();
            },
            getIndex = () => Array.IndexOf(
                phase.enumValues,
                (int)marketData.currentPhase),
            setIndex = _ => { }
        };
        data.children.Add(phase);
        data.children.Add(new DebugUI.IntField
        {
            displayName = "Current Level",
            getter = () => marketData.currentLevel,
            setter = value =>
            {
                marketData.currentLevel = value;
                LevelRefresh();
                NotifyMarketDataChanged();
            }
        });
        data.children.Add(new DebugUI.IntField
        {
            displayName = "Total Income",
            getter = () => marketData.totalIncome,
            setter = value =>
            {
                marketData.totalIncome = value;
                NotifyMarketDataChanged();
            }
        });
        data.children.Add(new DebugUI.IntField
        {
            displayName = "Yesterday Sales",
            getter = () => marketData.yesterdaySales,
            setter = value =>
            {
                marketData.yesterdaySales = value;
                NotifyMarketDataChanged();
            }
        });
        debugPanel.children.Add(data);

        DebugUI.Foldout selectedDishes = new()
        {
            displayName = "Selected Dishes",
            opened = false
        };
        for (int i = 0; i < marketData.selectedDishes.Count; i++)
        {
            int index = i;
            DebugUI.Foldout row = new()
            {
                displayName = $"[{index}]",
                opened = false
            };

            DebugUI.EnumField dish = null;
            dish = new DebugUI.EnumField
            {
                displayName = "Dish",
                autoEnum = typeof(DishType),
                getter = () => (int)marketData.selectedDishes[index],
                setter = value =>
                {
                    marketData.selectedDishes[index] = (DishType)value;
                    NotifyMarketDataChanged();
                },
                getIndex = () => Array.IndexOf(
                    dish.enumValues,
                    (int)marketData.selectedDishes[index]),
                setIndex = _ => { }
            };
            row.children.Add(dish);
            row.children.Add(new DebugUI.Button
            {
                displayName = "Remove",
                action = () =>
                {
                    marketData.selectedDishes.RemoveAt(index);
                    NotifyMarketDataChanged();
                    RebuildDebugUI();
                }
            });
            selectedDishes.children.Add(row);
        }
        selectedDishes.children.Add(new DebugUI.Button
        {
            displayName = "Add Entry",
            action = () =>
            {
                marketData.selectedDishes.Add(DishType.None);
                NotifyMarketDataChanged();
                RebuildDebugUI();
            }
        });
        debugPanel.children.Add(selectedDishes);

        DebugUI.Foldout currentLevelData = new()
        {
            displayName = "Level Data",
            opened = false
        };
        currentLevelData.children.Add(new DebugUI.IntField
        {
            displayName = "Level",
            getter = () => levelData.level,
            setter = value =>
            {
                levelData.level = value;
                NotifyMarketDataChanged();
            }
        });
        currentLevelData.children.Add(new DebugUI.IntField
        {
            displayName = "Max Dish Limit",
            getter = () => levelData.maxDishLimit,
            setter = value =>
            {
                levelData.maxDishLimit = value;
                NotifyMarketDataChanged();
            }
        });
        currentLevelData.children.Add(new DebugUI.IntField
        {
            displayName = "Income Goal",
            getter = () => levelData.incomeGoal,
            setter = value =>
            {
                levelData.incomeGoal = value;
                NotifyMarketDataChanged();
            }
        });
        currentLevelData.children.Add(new DebugUI.IntField
        {
            displayName = "Base Customer Count",
            getter = () => levelData.baseCustomerCount,
            setter = value =>
            {
                levelData.baseCustomerCount = value;
                NotifyMarketDataChanged();
            }
        });
        debugPanel.children.Add(currentLevelData);

        DebugUI.Foldout festivals = new()
        {
            displayName = "Festival Calendar",
            opened = false
        };

        DebugUI.EnumField taste = null;
        taste = new DebugUI.EnumField
        {
            displayName = "Latest Taste",
            autoEnum = typeof(TasteType),
            getter = () => (int)festivalCalendar.latestTaste,
            setter = value =>
            {
                festivalCalendar.latestTaste = (TasteType)value;
                NotifyMarketDataChanged();
            },
            getIndex = () => Array.IndexOf(
                taste.enumValues,
                (int)festivalCalendar.latestTaste),
            setIndex = _ => { }
        };
        festivals.children.Add(taste);
        festivals.children.Add(new DebugUI.IntField
        {
            displayName = "Taste Start Business Day",
            getter = () => festivalCalendar.tasteStartBusinessDay,
            setter = value =>
            {
                festivalCalendar.tasteStartBusinessDay = value;
                NotifyMarketDataChanged();
            }
        });
        festivals.children.Add(new DebugUI.Value
        {
            displayName = "Taste End Business Day",
            getter = () => festivalCalendar.TasteEndBusinessDay
        });

        DebugUI.EnumField category = null;
        category = new DebugUI.EnumField
        {
            displayName = "Latest Category",
            autoEnum = typeof(CategoryType),
            getter = () => (int)festivalCalendar.latestCategory,
            setter = value =>
            {
                festivalCalendar.latestCategory = (CategoryType)value;
                NotifyMarketDataChanged();
            },
            getIndex = () => Array.IndexOf(
                category.enumValues,
                (int)festivalCalendar.latestCategory),
            setIndex = _ => { }
        };
        festivals.children.Add(category);
        festivals.children.Add(new DebugUI.IntField
        {
            displayName = "Category Start Business Day",
            getter = () => festivalCalendar.categoryStartBusinessDay,
            setter = value =>
            {
                festivalCalendar.categoryStartBusinessDay = value;
                NotifyMarketDataChanged();
            }
        });
        festivals.children.Add(new DebugUI.Value
        {
            displayName = "Category End Business Day",
            getter = () => festivalCalendar.CategoryEndBusinessDay
        });
        debugPanel.children.Add(festivals);

        DebugUI.Foldout mission = new()
        {
            displayName = "Mission State",
            opened = false
        };
        mission.children.Add(new DebugUI.Value
        {
            displayName = "Claimed Mission Count",
            getter = () => levelMissionProgress.ClaimedMissionCount
        });
        mission.children.Add(new DebugUI.Value
        {
            displayName = "Current Mission",
            getter = () => levelMissionProgress.CurrentMission == null
                ? "None"
                : levelMissionProgress.CurrentMission.ToString()
        });
        mission.children.Add(new DebugUI.Value
        {
            displayName = "Satisfied",
            getter = () => levelMissionProgress.IsCurrentMissionSatisfied
        });
        mission.children.Add(new DebugUI.Value
        {
            displayName = "Can Claim Reward",
            getter = () => levelMissionProgress.CanClaimCurrentReward
        });
        mission.children.Add(new DebugUI.Value
        {
            displayName = "Can Promote",
            getter = () => CanPromote
        });
        debugPanel.children.Add(mission);
        DebugManager.instance.ReDrawOnScreenDebug();
    }
}
