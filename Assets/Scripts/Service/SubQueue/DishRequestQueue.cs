using System.Collections.Generic;
using UnityEngine;

public class DishRequestQueue : MonoBehaviour
{
    [SerializeField]private Queue<DishType> queue = new();
    public Queue<DishType> Queue => queue;
}
