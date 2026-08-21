using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishEffectQueue : MonoBehaviour
{
    private Queue<DishType> tipChanceUpQueue = new();
    private Queue<DishType> eatSpeedUpQueue = new();
    public Queue<DishType> TipChanceUpQueue => tipChanceUpQueue;
    public Queue<DishType> EatSpeedUpQueue => eatSpeedUpQueue;

    
}
