using System;
using UnityEngine;

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
