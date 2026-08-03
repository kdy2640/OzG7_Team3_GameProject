using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private Transform waypointParent;

    public List<Waypoint> AllWaypoints { get; private set; }

    private void Awake()
    {
        AllWaypoints = new List<Waypoint>();

        foreach (Transform child in waypointParent)
        {
            AllWaypoints.Add(child.GetComponent<Waypoint>());
        }

    }

    public Waypoint GetClosestWaypoint(Vector3 position)
    {
        Waypoint closest = null;
        float minDistance = float.MaxValue;

        foreach (Waypoint waypoint in AllWaypoints)
        {
            float distance =
                (waypoint.transform.position - position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = waypoint;
            }
        }

        return closest;
    }
}
