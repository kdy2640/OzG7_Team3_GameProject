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
        Debug.Log("손님 퇴장 완료");
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        GameObject.Destroy(stateManager);
    }
}