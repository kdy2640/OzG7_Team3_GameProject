
using UnityEngine;

public class ServerMoveToKitchenState : IState
{
    private ServerStateManager stateManager;
    private AIMove aiMove;
    private Transform kitchen;

    public ServerMoveToKitchenState(
        ServerStateManager stateManager,
        AIMove aiMove,
        Transform kitchen)
    {
        this.stateManager = stateManager;
        this.aiMove = aiMove;
        this.kitchen = kitchen;
    }


    public void Enter()
    {
        stateManager.Renderer.material.color = Color.ivory;
        stateManager.IsBusy = true;

        aiMove.OnArrived += Arrived;

        aiMove.MoveTo(kitchen);
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
            new ServerMoveToTableState(stateManager, stateManager.AiMove, stateManager.ServePoint)
            );
    }
}