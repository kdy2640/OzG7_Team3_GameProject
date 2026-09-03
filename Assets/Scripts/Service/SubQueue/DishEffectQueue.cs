using System.Collections.Generic;
using UnityEngine;

public class DishEffectQueue : MonoBehaviour
{
    private readonly Dictionary<DishType, int> tipChanceUpCounts = new();
    private readonly Dictionary<DishType, int> eatSpeedUpCounts = new();

    public void AddTipChanceUp(DishType dish)
    {
        if (tipChanceUpCounts.TryGetValue(dish, out int count))
        {
            tipChanceUpCounts[dish] = count + 1;
        }
        else
        {
            tipChanceUpCounts.Add(dish, 1);
        }
    }

    public void AddEatSpeedUp(DishType dish)
    {
        if (eatSpeedUpCounts.TryGetValue(dish, out int count))
        {
            eatSpeedUpCounts[dish] = count + 1;
        }
        else
        {
            eatSpeedUpCounts.Add(dish, 1);
        }
    }

    public bool TryConsumeTipChanceUp(DishType dish)
    {
        if (!tipChanceUpCounts.TryGetValue(dish, out int count)
            || count <= 0)
        {
            return false;
        }

        if (count == 1)
        {
            tipChanceUpCounts.Remove(dish);
        }
        else
        {
            tipChanceUpCounts[dish] = count - 1;
        }

        return true;
    }

    public bool TryConsumeEatSpeedUp(DishType dish)
    {
        if (!eatSpeedUpCounts.TryGetValue(dish, out int count)
            || count <= 0)
        {
            return false;
        }

        if (count == 1)
        {
            eatSpeedUpCounts.Remove(dish);
        }
        else
        {
            eatSpeedUpCounts[dish] = count - 1;
        }

        return true;
    }
}
