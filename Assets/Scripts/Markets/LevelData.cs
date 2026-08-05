using System;
using UnityEngine;

[Serializable]
public sealed class LevelData
{
    [SerializeField, Min(0)] private int level;
    [SerializeField, Min(0)] private int maxDishLimit;
    [SerializeField, Min(0)] private int maxEXPLimit;
    [SerializeField, Min(0f)] private float unlockedChef;
    [SerializeField, Min(0f)] private float unlockedServer;
    [SerializeField, Min(0f)] private float unlockedHarvester;
    [SerializeField, Min(0f)] private float unlockedTable;
    [SerializeField, Min(0f)] private float unlockedDeco;

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

    public float UnlockedChef
    {
        get => unlockedChef;
        set => unlockedChef = Mathf.Max(0f, value);
    }

    public float UnlockedServer
    {
        get => unlockedServer;
        set => unlockedServer = Mathf.Max(0f, value);
    }

    public float UnlockedHarvester
    {
        get => unlockedHarvester;
        set => unlockedHarvester = Mathf.Max(0f, value);
    }

    public float UnlockedTable
    {
        get => unlockedTable;
        set => unlockedTable = Mathf.Max(0f, value);
    }

    public float UnlockedDeco
    {
        get => unlockedDeco;
        set => unlockedDeco = Mathf.Max(0f, value);
    }
}
