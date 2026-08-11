using UnityEngine;

public class ServerGetBackState : IState
{
    private ServerStateManager stateManager;
    private AIMove aiMove;

    private Transform exitPoint;


    public ServerGetBackState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        stateManager.IsBusy = false;

        stateManager.Renderer.material.color = Color.black;

        stateManager.AiMove.OnArrived += ArrivedHome;

        stateManager.AiMove.MoveTo(
            stateManager.WaitPoint
        );
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        stateManager.AiMove.OnArrived -= ArrivedHome;
    }


    private void ArrivedHome()
    {
        stateManager.ChangeState(
            new ServerIdleState(stateManager)
        );
    }
}