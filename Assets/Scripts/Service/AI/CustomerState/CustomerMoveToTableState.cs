using UnityEngine;

public class CustomerMoveToTableState : IState
{
    private CustomerStateManager stateManager;
    private AIMove aiMove;
    private Transform seat;


    public CustomerMoveToTableState(
        CustomerStateManager stateManager,
        AIMove aiMove,
        Transform seat)
    {
        this.stateManager = stateManager;
        this.aiMove = aiMove;
        this.seat = seat;
    }


    public void Enter()
    {
        stateManager.Renderer.material.color = Color.red;
        aiMove.OnArrived += Arrived;

        aiMove.MoveTo(seat);
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        aiMove.OnArrived -= Arrived;
    }


    private void Arrived()
    {
        stateManager.ChangeState(
            new CustomerOrderState(stateManager)
        );
    }
}
