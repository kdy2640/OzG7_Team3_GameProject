using UnityEngine;

public class CustomerGoHomeState : IState
{
    private CustomerStateManager stateManager;
    private AIMove aiMove;

    private Transform exitPoint;


    public CustomerGoHomeState(CustomerStateManager stateManager)
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
            new CustomerEndState(stateManager)
        );
    }
}