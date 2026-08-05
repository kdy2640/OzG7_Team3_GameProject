using UnityEngine;

public class CustomerEndState : IState
{
    private CustomerStateManager stateManager;

    public CustomerEndState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        GameObject.Destroy(stateManager);
    }
}