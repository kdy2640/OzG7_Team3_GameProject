using System;
using UnityEngine;

[Serializable]
public struct RuntimeLevelViewer
{
    [SerializeField] private string targetName;
    [SerializeField] private int level;

    public int Level => level;

    internal void SetName(string name)
    {
        targetName = name;
    }

    internal void SetLevel(int newLevel)
    {
        level = Mathf.Max(0, newLevel);
    }
}

[Serializable]
public sealed class RuntimeLevel
{
    [SerializeField] private RuntimeLevelViewer[] harvestLevels = Array.Empty<RuntimeLevelViewer>();
    [SerializeField] private RuntimeLevelViewer[] dishLevels = Array.Empty<RuntimeLevelViewer>();
    [SerializeField] private RuntimeLevelViewer[] employeeLevels = Array.Empty<RuntimeLevelViewer>();
    [SerializeField] private RuntimeLevelViewer[] facilityLevels = Array.Empty<RuntimeLevelViewer>();

    public RuntimeLevel()
    {
        EnsureCapacity();
    }

    public int Get(HarvestUpgradeType harvestUpgradeType)
    {
        int index = (int)harvestUpgradeType;
        if (index < 0 || index >= (int)HarvestUpgradeType.Count)
            return 0;

        EnsureCapacity();
        return harvestLevels[index].Level;
    }

    public int Get(DishType dishType)
    {
        int index = (int)dishType;
        if (index < 0 || index >= (int)DishType.Count)
            return 0;

        EnsureCapacity();
        return dishLevels[index].Level;
    }

    public int Get(EmployeeType employeeType)
    {
        int index = (int)employeeType;
        if (index < 0 || index >= (int)EmployeeType.Count)
            return 0;

        EnsureCapacity();
        return employeeLevels[index].Level;
    }

    public int Get(FacilityType facilityType)
    {
        int index = (int)facilityType;
        if (index < 0 || index >= (int)FacilityType.Count)
            return 0;

        EnsureCapacity();
        return facilityLevels[index].Level;
    }

    internal void Set(HarvestUpgradeType harvestUpgradeType, int level)
    {
        int index = (int)harvestUpgradeType;
        if (index < 0 || index >= (int)HarvestUpgradeType.Count)
            return;

        EnsureCapacity();
        harvestLevels[index].SetLevel(level);
    }

    internal void Set(DishType dishType, int level)
    {
        int index = (int)dishType;
        if (index < 0 || index >= (int)DishType.Count)
            return;

        EnsureCapacity();
        dishLevels[index].SetLevel(level);
    }

    internal void Set(EmployeeType employeeType, int level)
    {
        int index = (int)employeeType;
        if (index < 0 || index >= (int)EmployeeType.Count)
            return;

        EnsureCapacity();
        employeeLevels[index].SetLevel(level);
    }

    internal void Set(FacilityType facilityType, int level)
    {
        int index = (int)facilityType;
        if (index < 0 || index >= (int)FacilityType.Count)
            return;

        EnsureCapacity();
        facilityLevels[index].SetLevel(level);
    }

    private void EnsureCapacity()
    {
        harvestLevels ??= Array.Empty<RuntimeLevelViewer>();
        if (harvestLevels.Length != (int)HarvestUpgradeType.Count)
            Array.Resize(ref harvestLevels, (int)HarvestUpgradeType.Count);

        for (int i = 0; i < harvestLevels.Length; i++)
            harvestLevels[i].SetName(((HarvestUpgradeType)i).ToString());

        dishLevels ??= Array.Empty<RuntimeLevelViewer>();
        if (dishLevels.Length != (int)DishType.Count)
            Array.Resize(ref dishLevels, (int)DishType.Count);

        for (int i = 0; i < dishLevels.Length; i++)
            dishLevels[i].SetName(((DishType)i).ToString());

        employeeLevels ??= Array.Empty<RuntimeLevelViewer>();
        if (employeeLevels.Length != (int)EmployeeType.Count)
            Array.Resize(ref employeeLevels, (int)EmployeeType.Count);

        for (int i = 0; i < employeeLevels.Length; i++)
            employeeLevels[i].SetName(((EmployeeType)i).ToString());

        facilityLevels ??= Array.Empty<RuntimeLevelViewer>();
        if (facilityLevels.Length != (int)FacilityType.Count)
            Array.Resize(ref facilityLevels, (int)FacilityType.Count);

        for (int i = 0; i < facilityLevels.Length; i++)
            facilityLevels[i].SetName(((FacilityType)i).ToString());
    }
}
