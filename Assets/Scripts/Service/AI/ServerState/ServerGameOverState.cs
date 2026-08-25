using UnityEngine;

public class ServerGameOverState : IState
{
    private ServerStateManager stateManager;

    public ServerGameOverState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }
    public void Enter()
    {
        stateManager.AiMove.StopMove();
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}
