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
        stateManager.AiMove.OnArrived += ArrivedHome;

        stateManager.AiMove.MoveTo(
            stateManager.ExitPoint
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
            new ServerEndState(stateManager)
        );
    }
}