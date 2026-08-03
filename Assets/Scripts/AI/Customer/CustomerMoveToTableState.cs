using UnityEngine;

public class CustomerMoveToTableState : IState
{
    private CustomerStateManager stateManager;
    private AIMove aiMove;
    private Transform table;


    public CustomerMoveToTableState(
        CustomerStateManager stateManager,
        AIMove aiMove,
        Transform table)
    {
        this.stateManager = stateManager;
        this.aiMove = aiMove;
        this.table = table;
    }


    public void Enter()
    {
        aiMove.OnArrived += Arrived;

        aiMove.MoveTo(table);
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
            new CustomerEatState(stateManager)
        );
    }
}
