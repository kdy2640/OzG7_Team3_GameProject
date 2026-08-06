using UnityEngine;

public class ServerMoveToTableState : IState
{
    private ServerStateManager stateManager;
    private AIMove aiMove;
    private Transform table;


    public ServerMoveToTableState(
        ServerStateManager stateManager,
        AIMove aiMove,
        Transform table)
    {
        this.stateManager = stateManager;
        this.aiMove = aiMove;
        this.table = table;
    }


    public void Enter()
    {
        aiMove.OnArrived += Arrived;

        aiMove.MoveTo(table);
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        aiMove.OnArrived -= Arrived;
    }


    private void Arrived()
    {
        stateManager.ChangeState(
            new ServerServeState(stateManager)
        );
    }
}