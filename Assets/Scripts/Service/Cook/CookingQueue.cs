using System.Collections.Generic;
using UnityEngine;

public class CookingQueue : MonoBehaviour
{
    private Queue<DishType> cookingQueue = new();
    private DishType currentDish;
    private float timer;

    private void Update()
    {
        if(currentDish == DishType.Count)
        {
            StartNextCooking();
            return;
        }

        timer -= Time.deltaTime;

        if(timer<= 0)
        {
            FinishCooking();
        }
    }

    public void RequestCook(DishType dish)
    {
        cookingQueue.Enqueue(dish);
    }

    private void StartNextCooking()
    {
        if(cookingQueue.Count == 0)
        {
            return;
        }

        currentDish = cookingQueue.Dequeue();

        timer = 3f;
    }

    private void FinishCooking()
    {
        
    }
}
