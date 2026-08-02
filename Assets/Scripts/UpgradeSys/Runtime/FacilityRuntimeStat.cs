using System;
using UnityEngine;


[Serializable]
public sealed class FacilityRuntimeStat
{
    [SerializeField] private int[] levels = Array.Empty<int>();

    public int GetLevel(FacilityType facilityType)
    {
        int index = (int)facilityType;

        if (!IsValidIndex(index))
            return 0;

        EnsureCapacity();
        return Mathf.Max(0, levels[index]);
    }

    internal void Apply(FacilityType facilityType, int level)
    {
        int index = (int)facilityType;

        if (!IsValidIndex(index))
            return;

        EnsureCapacity();
        levels[index] = Mathf.Max(0, level);
    }

    private static bool IsValidIndex(int index)
    {
        return index >= 0 && index < (int)FacilityType.Count;
    }

    private void EnsureCapacity()
    {
        levels ??= Array.Empty<int>();

        if (levels.Length != (int)FacilityType.Count)
            Array.Resize(ref levels, (int)FacilityType.Count);
    }
}
