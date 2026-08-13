using System;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    private NavMeshAgent agent;

    public event Action OnArrived;

    private bool isMoving;
    private bool isRotating;

    private Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Transform target)
    {
        this.target = target;
        agent.SetDestination(target.position);
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (agent.pathPending)
                return;

            if (agent.remainingDistance <= 0.5f)
            {
                isMoving = false;
                isRotating = true;
            }
        }

        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                target.rotation,
                360f * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, target.rotation) < 0.1f)
            {
                isRotating = false;
                OnArrived?.Invoke();
            }
        }
    }
}