using System;
using UnityEngine;

[Serializable]
public sealed class DishRuntimeStat
{
    [SerializeField] private int[] levels = Array.Empty<int>();

    public int GetLevel(DishType dishType)
    {
        int index = (int)dishType;

        if (!IsValidIndex(index))
            return 0;

        EnsureCapacity();
        return Mathf.Max(0, levels[index]);
    }

    internal void Apply(DishType dishType, int level)
    {
        int index = (int)dishType;

        if (!IsValidIndex(index))
            return;

        EnsureCapacity();
        levels[index] = Mathf.Max(0, level);
    }

    private static bool IsValidIndex(int index)
    {
        return index >= 0 && index < (int)DishType.Count;
    }

    private void EnsureCapacity()
    {
        levels ??= Array.Empty<int>();

        if (levels.Length != (int)DishType.Count)
            Array.Resize(ref levels, (int)DishType.Count);
    }
}
