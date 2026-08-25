using UnityEngine;

public class CustomerGameOverState : IState
{
    private CustomerStateManager stateManager;

    public CustomerGameOverState(CustomerStateManager stateManager)
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
