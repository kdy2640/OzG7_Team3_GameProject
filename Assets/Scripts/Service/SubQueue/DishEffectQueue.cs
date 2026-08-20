using System.Collections.Generic;
using UnityEngine;

public class DishEffectQueue : MonoBehaviour
{
    private Queue<DishType> queue = new();
    public Queue<DishType> Queue => queue;
}
