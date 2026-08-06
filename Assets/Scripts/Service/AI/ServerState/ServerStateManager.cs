using UnityEngine;

public class ServerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform table;
    [SerializeField] private Transform exitPoint;

    public AIMove AiMove => aiMove;
    public Transform ExitPoint => exitPoint;

    private IState currentState;

    private void Start()
    {
        ChangeState(new ServerMoveToTableState(this, aiMove, table));
    }

    private void Update()
    {
        currentState?.Execute();
    }


    public void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }
}