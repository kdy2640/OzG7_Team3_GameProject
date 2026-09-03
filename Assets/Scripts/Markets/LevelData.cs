using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class LevelData
{
    [SerializeField, Min(0)] internal int level;
    [SerializeField, Min(0)] internal int maxDishLimit;
    [FormerlySerializedAs("maxEXPLimit")]
    [SerializeField, Min(0)] internal int incomeGoal;
    [SerializeField, Min(0)] internal int baseCustomerCount;

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

    public int IncomeGoal
    {
        get => incomeGoal;
        set => incomeGoal = Mathf.Max(0, value);
    }

    public int BaseCustomerCount
    {
        get => baseCustomerCount;
        set => baseCustomerCount = Mathf.Max(0, value);
    }
}
