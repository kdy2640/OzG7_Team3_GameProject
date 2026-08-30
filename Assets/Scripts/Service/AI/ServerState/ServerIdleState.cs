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
        if (stateManager.Customer != null)
        {
            stateManager.AiMove.SetDirection(stateManager.Customer.transform.position);
        }
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

