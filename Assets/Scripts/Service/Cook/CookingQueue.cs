using System.Collections.Generic;
using UnityEngine;

public class CookingQueue : MonoBehaviour
{
    [SerializeField] private int queueMaxCount = 2;

    private Queue<DishType> cookingQueue = new();
    
    public int Count => cookingQueue.Count;

    public bool CanRequestCook()
    {
        return cookingQueue.Count < queueMaxCount;
    }

    public bool RequestCook(DishType dish)
    {
        if(!CanRequestCook())
        {
            return false;
        }
        cookingQueue.Enqueue(dish);
        return true;
    }

    public bool TryGetNextDish(out DishType dish)
    {
        if(cookingQueue == null)
        {
            Debug.LogWarning("cookingQUeueNull");
            dish = DishType.Count;
            return false;
        }
        if(cookingQueue.Count > 0)
        {
            dish = cookingQueue.Dequeue();
            return true;
        }

        dish = DishType.Count;
        return false;
    }
}
