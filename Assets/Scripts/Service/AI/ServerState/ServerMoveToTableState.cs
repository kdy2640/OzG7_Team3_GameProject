using UnityEngine;

public class ServerMoveToTableState : IState
{
    private ServerStateManager stateManager;
    private AIMove aiMove;
    private Transform servePoint;


    public ServerMoveToTableState(
        ServerStateManager stateManager,
        AIMove aiMove,
        Transform servePoint)
    {
        this.stateManager = stateManager;
        this.aiMove = aiMove;
        this.servePoint = servePoint;
    }


    public void Enter()
    {
        stateManager.animator.SetBool("IsServing", true);

        stateManager.IsBusy = true;

        aiMove.OnArrived += Arrived;

        aiMove.MoveTo(servePoint);
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        stateManager.animator.SetBool("IsServing", false); 
        aiMove.OnArrived -= Arrived;
        
    }


    private void Arrived()
    {
        stateManager.ChangeState(
            new ServerServeState(stateManager)
        );
    }
}