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

        stateManager.IsBusy = false;
        stateManager.Renderer.material.color = Color.white;

    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}

