#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class UpgradeManager
{
    private const string DebugPanelName = "Game / Upgrade";

    private DebugUI.Panel debugPanel;
    private readonly List<UpgradeDataSO> debugUpgradeData = new();

    internal void RegisterDebugUI()
    {
        debugUpgradeData.Clear();
        debugUpgradeData.AddRange(
            Resources.LoadAll<UpgradeDataSO>("SOs/UpgradeDatas/Harvest"));
        debugUpgradeData.AddRange(
            Resources.LoadAll<UpgradeDataSO>("SOs/UpgradeDatas/Dish"));
        debugUpgradeData.AddRange(
            Resources.LoadAll<UpgradeDataSO>("SOs/UpgradeDatas/Employee"));
        debugUpgradeData.AddRange(
            Resources.LoadAll<UpgradeDataSO>("SOs/UpgradeDatas/Facility"));

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
            displayName = "Runtime Level and Runtime Stat are temporary overrides. Recalculation, loading, or related market changes overwrite them.",
            style = DebugUI.MessageBox.Style.Warning
        });

        DebugUI.Foldout states = new()
        {
            displayName = "Upgrade States",
            opened = false
        };

        for (int i = 0; i < upgradeStates.Count; i++)
        {
            int index = i;
            UpgradeState state = upgradeStates[index];
            DebugUI.Foldout row = new()
            {
                displayName = $"[{index}]",
                opened = false
            };

            if (state == null)
            {
                row.children.Add(new DebugUI.MessageBox
                {
                    displayName = "null entry",
                    style = DebugUI.MessageBox.Style.Error
                });
            }
            else
            {
                row.children.Add(new DebugUI.ObjectPopupField
                {
                    displayName = "Data",
                    getter = () => state.data,
                    setter = value =>
                    {
                        state.data = (UpgradeDataSO)value;
                        BuildUpgradeStateMaps();
                        RefreshRuntimeData();
                        RebuildDebugUI();
                    },
                    getObjects = () => debugUpgradeData
                });
                row.children.Add(new DebugUI.Value
                {
                    displayName = "Id",
                    getter = () => state.data == null
                        ? "null"
                        : state.data.Id
                });
                row.children.Add(new DebugUI.IntField
                {
                    displayName = "Level",
                    getter = () => state.level,
                    setter = value =>
                    {
                        state.level = value;
                        RefreshRuntimeData();
                    }
                });
            }

            row.children.Add(new DebugUI.Button
            {
                displayName = "Remove",
                action = () =>
                {
                    upgradeStates.RemoveAt(index);
                    BuildUpgradeStateMaps();
                    RefreshRuntimeData();
                    RebuildDebugUI();
                }
            });
            states.children.Add(row);
        }

        states.children.Add(new DebugUI.Button
        {
            displayName = "Add Entry",
            action = () =>
            {
                upgradeStates.Add(new UpgradeState());
                RebuildDebugUI();
            }
        });
        states.children.Add(new DebugUI.Button
        {
            displayName = "Recalculate From Upgrade States",
            action = () =>
            {
                BuildUpgradeStateMaps();
                RefreshRuntimeData();
                RebuildDebugUI();
            }
        });
        debugPanel.children.Add(states);

        DebugUI.Foldout runtimeLevels = new()
        {
            displayName = "Runtime Level",
            opened = false
        };

        DebugUI.Foldout harvestLevels = new()
        {
            displayName = "Harvest",
            opened = false
        };
        for (int i = 0; i < (int)HarvestUpgradeType.Count; i++)
        {
            HarvestUpgradeType type = (HarvestUpgradeType)i;
            harvestLevels.children.Add(new DebugUI.IntField
            {
                displayName = type.ToString(),
                getter = () => runtimeLevel.Get(type),
                setter = value =>
                {
                    runtimeLevel.Set(type, value);
                    onUpgradeChanged?.Invoke();
                }
            });
        }
        runtimeLevels.children.Add(harvestLevels);

        DebugUI.Foldout dishLevels = new()
        {
            displayName = "Dish",
            opened = false
        };
        for (int i = 0; i < (int)DishType.Count; i++)
        {
            DishType type = (DishType)i;
            dishLevels.children.Add(new DebugUI.IntField
            {
                displayName = type.ToString(),
                getter = () => runtimeLevel.Get(type),
                setter = value =>
                {
                    runtimeLevel.Set(type, value);
                    onUpgradeChanged?.Invoke();
                }
            });
        }
        runtimeLevels.children.Add(dishLevels);

        DebugUI.Foldout employeeLevels = new()
        {
            displayName = "Employee",
            opened = false
        };
        for (int i = 0; i < (int)EmployeeType.Count; i++)
        {
            EmployeeType type = (EmployeeType)i;
            employeeLevels.children.Add(new DebugUI.IntField
            {
                displayName = type.ToString(),
                getter = () => runtimeLevel.Get(type),
                setter = value =>
                {
                    runtimeLevel.Set(type, value);
                    onUpgradeChanged?.Invoke();
                }
            });
        }
        runtimeLevels.children.Add(employeeLevels);

        DebugUI.Foldout facilityLevels = new()
        {
            displayName = "Facility",
            opened = false
        };
        for (int i = 0; i < (int)FacilityType.Count; i++)
        {
            FacilityType type = (FacilityType)i;
            facilityLevels.children.Add(new DebugUI.IntField
            {
                displayName = type.ToString(),
                getter = () => runtimeLevel.Get(type),
                setter = value =>
                {
                    runtimeLevel.Set(type, value);
                    onUpgradeChanged?.Invoke();
                }
            });
        }
        runtimeLevels.children.Add(facilityLevels);
        debugPanel.children.Add(runtimeLevels);

        DebugUI.Foldout runtimeStats = new()
        {
            displayName = "Runtime Stat",
            opened = false
        };
        DebugUI.Foldout harvestStats = new()
        {
            displayName = "Harvest",
            opened = false
        };
        for (int i = 0; i < (int)HarvestStatType.Count; i++)
        {
            HarvestStatType type = (HarvestStatType)i;
            harvestStats.children.Add(new DebugUI.FloatField
            {
                displayName = type.ToString(),
                getter = () => runtimeStat.Harvest.Get(type),
                setter = value =>
                {
                    runtimeStat.Harvest.Set(type, value);
                    onUpgradeChanged?.Invoke();
                },
                min = () => 0f
            });
        }
        runtimeStats.children.Add(harvestStats);

        DebugUI.Foldout serviceStats = new()
        {
            displayName = "Service",
            opened = false
        };
        for (int i = 0; i < (int)ServiceStatType.Count; i++)
        {
            ServiceStatType type = (ServiceStatType)i;
            serviceStats.children.Add(new DebugUI.FloatField
            {
                displayName = type.ToString(),
                getter = () => runtimeStat.Service.Get(type),
                setter = value =>
                {
                    runtimeStat.Service.Set(type, value);
                    onUpgradeChanged?.Invoke();
                },
                min = () => 0f
            });
        }
        runtimeStats.children.Add(serviceStats);
        debugPanel.children.Add(runtimeStats);
        DebugManager.instance.ReDrawOnScreenDebug();
    }
}
#endif
