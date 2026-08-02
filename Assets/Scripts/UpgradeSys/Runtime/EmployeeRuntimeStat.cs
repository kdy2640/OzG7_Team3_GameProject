using System;
using UnityEngine;


[Serializable]
public sealed class EmployeeRuntimeStat
{
    [SerializeField] private int[] levels = Array.Empty<int>();

    public int GetLevel(EmployeeType employeeType)
    {
        int index = (int)employeeType;

        if (!IsValidIndex(index))
            return 0;

        EnsureCapacity();
        return Mathf.Max(0, levels[index]);
    }

    internal void Apply(EmployeeType employeeType, int level)
    {
        int index = (int)employeeType;

        if (!IsValidIndex(index))
            return;

        EnsureCapacity();
        levels[index] = Mathf.Max(0, level);
    }

    private static bool IsValidIndex(int index)
    {
        return index >= 0 && index < (int)EmployeeType.Count;
    }

    private void EnsureCapacity()
    {
        levels ??= Array.Empty<int>();

        if (levels.Length != (int)EmployeeType.Count)
            Array.Resize(ref levels, (int)EmployeeType.Count);
    }
}
