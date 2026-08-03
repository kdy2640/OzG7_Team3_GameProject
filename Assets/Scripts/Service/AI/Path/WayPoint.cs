using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private List<Waypoint> neighbors = new();
    public IReadOnlyList<Waypoint> Neighbors => neighbors;

    public Waypoint Parent;

    public int GCost;
    public int HCost;

    public int FCost => GCost + HCost;
}