using System;
using UnityEngine;

public enum ModifierType
{
    Add,
    Multiply,
    Max
}


[Serializable]
public sealed class RuntimeStat
{
    [SerializeField] private HarvestRuntimeStat harvest = new();
    [SerializeField] private ServiceRuntimeStat service = new();

    public HarvestRuntimeStat Harvest => harvest ??= new HarvestRuntimeStat();
    public ServiceRuntimeStat Service => service ??= new ServiceRuntimeStat();
}
