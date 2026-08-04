
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder
{

    private GraphManager graph;

    public AStarPathFinder(GraphManager graph)
    {
        this.graph = graph;
    }


    public List<Waypoint> FindPath(Waypoint start, Waypoint goal)
    {
        List<Waypoint> openList = new();
        HashSet<Waypoint> closedSet = new();
        openList.Add(start);

        foreach (Waypoint node in graph.AllWaypoints)
        {
            node.GCost = int.MaxValue;
            node.HCost = 0;
            node.Parent = null;
        }

        while (openList.Count > 0)
        {
            Waypoint current = openList[0];


            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < current.FCost)
                {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedSet.Add(current);

            if (current == goal)
            {
                return RetracePath(start, goal);
            }

            

            start.GCost = 0;

            foreach (Waypoint neighbor in current.Neighbors)
            {

                if (closedSet.Contains(neighbor))
                    continue;

                int newGCost = current.GCost + 1;

                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetDistance(neighbor, goal);
                    neighbor.Parent = current;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }


        return null;
    }

    public int GetDistance(Waypoint a, Waypoint b)
    {
        return Mathf.RoundToInt(Vector3.Distance(a.transform.position, b.transform.position));
    }

    // 경로 반환
    private List<Waypoint> RetracePath(Waypoint start, Waypoint end)
    {
        List<Waypoint> path = new();

        Waypoint current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Add(start);

        path.Reverse();

        return path;
    }
}
