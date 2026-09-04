using UnityEngine;

public class ServerIdleState : IState
{
    private ServerStateManager stateManager;

    public ServerIdleState(ServerStateManager stateManager)
    {

        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.AiMove.SetDirectionVector(Vector3.back);
        stateManager.AnimSetIdle();
        stateManager.IsBusy = false;
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}

