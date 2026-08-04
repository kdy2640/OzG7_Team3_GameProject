using System;
using UnityEngine;

[Serializable]
public struct LevelStatViewer
{
    [SerializeField] private string statName;
    [SerializeField] private int value;

    public int Value => value;

    internal void SetName(string name)
    {
        statName = name;
    }

    internal void SetValue(int newValue)
    {
        value = newValue;
    }
}

[Serializable]
public struct HarvestStatViewer
{
    [SerializeField] private string statName;
    [SerializeField] private float value;

    public float Value => value;

    internal void SetName(string name)
    {
        statName = name;
    }

    internal void SetValue(float newValue)
    {
        value = newValue;
    }
}

[Serializable]
public sealed class RuntimeStat
{
    [SerializeField] private HarvestRuntimeStat harvest = new();
    [SerializeField] private DishRuntimeStat dish = new();
    [SerializeField] private EmployeeRuntimeStat employee = new();
    [SerializeField] private FacilityRuntimeStat facility = new();

    public HarvestRuntimeStat Harvest => harvest ??= new HarvestRuntimeStat();
    public DishRuntimeStat Dish => dish ??= new DishRuntimeStat();
    public EmployeeRuntimeStat Employee => employee ??= new EmployeeRuntimeStat();
    public FacilityRuntimeStat Facility => facility ??= new FacilityRuntimeStat();
}
