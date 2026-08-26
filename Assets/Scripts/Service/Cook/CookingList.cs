
using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingList : MonoBehaviour
{
    public event Action cookingListChanged;
    private readonly List<DishType> list = new();
    public IReadOnlyList<DishType> List => list;
    public void Add(DishType type)
    {
        list.Add(type);
        cookingListChanged?.Invoke();
    }
    public void Remove(DishType type)
    {
        if(list.Remove(type))
            cookingListChanged?.Invoke();
    }
}
