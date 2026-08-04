
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;



public class AIMove : MonoBehaviour
{
    public event Action OnArrived;

    private enum MoveState
    {
        ToStartWaypoint,
        FollowingPath,
        ToDestination,
        Arrived
    }

    private MoveState moveState;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private PathManager pathManager;
    [SerializeField] private GraphManager graph;


    
    private Transform destination;

    private List<Waypoint> currentPath;
    private int pathIndex;

    private Waypoint startWaypoint;

    private void Awake()
    {
        if(pathManager == null)
        {
            pathManager = FindFirstObjectByType<PathManager>();
        }

        if(graph == null)
        {
            graph = FindFirstObjectByType<GraphManager>();
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        switch (moveState)
        {
            case MoveState.ToStartWaypoint:
                MoveToNearWayPoint();
                break;

            case MoveState.FollowingPath:
                FollowPath();
                break;

            case MoveState.ToDestination:
                MoveToDestination();
                break;

            case MoveState.Arrived:
                break;
        }

    }

    private void MoveToNearWayPoint()
    {
        if(startWaypoint == null)
        {
            Debug.Log("startWaypoint = " + startWaypoint);
        }
        
        transform.position = Vector3.MoveTowards(
            transform.position,
            startWaypoint.transform.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, startWaypoint.transform.position) < 0.05f)
        {
            pathIndex = 0;
            moveState = MoveState.FollowingPath;
        }
    }

    private void FollowPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            moveState = MoveState.ToDestination;
            return;
        }

        Waypoint target = currentPath[pathIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentPath[pathIndex].transform.position,
            moveSpeed * Time.deltaTime );

        if (Vector3.Distance(transform.position, target.transform.position) < 0.05f)
        {
            pathIndex++;
        }

    }
    private void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination.position,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination.transform.position) < 0.05f)
        {
            moveState = MoveState.Arrived;

            OnArrived?.Invoke();

            return;
        }
    }

    public void MoveTo(Transform target)
    {
        destination = target;

        

        currentPath = pathManager.GetPath(transform.position, destination.position);

        if (currentPath == null)
        {
            startWaypoint = graph.GetClosestWaypoint(transform.position);
            return;
        }

        pathIndex = 0;

        startWaypoint = currentPath[0];

        moveState = MoveState.ToStartWaypoint;
    }

    public void StopMove()
    {
        moveState = MoveState.Arrived;
        currentPath = null;
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null)
            return;

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                currentPath[i].transform.position,
                currentPath[i + 1].transform.position
            );
        }
    }
}
