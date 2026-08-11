using System;
using UnityEngine;

[Serializable]
public sealed class LevelData
{
    [SerializeField, Min(0)] private int level;
    [SerializeField, Min(0)] private int maxDishLimit;
    [SerializeField, Min(0)] private int maxEXPLimit;

    public int Level
    {
        get => level;
        set => level = Mathf.Max(0, value);
    }

    public int MaxDishLimit
    {
        get => maxDishLimit;
        set => maxDishLimit = Mathf.Max(0, value);
    }

    public int MaxEXPLimit
    {
        get => maxEXPLimit;
        set => maxEXPLimit = Mathf.Max(0, value);
    }
}
