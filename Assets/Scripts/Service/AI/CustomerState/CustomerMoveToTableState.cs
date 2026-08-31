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
        stateManager.SetLifecycleProgress(0.1f);
        stateManager.Animator.SetBool("IsWalking", true);

        aiMove.OnArrived += Arrived;

        aiMove.MoveTo(seat);
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        stateManager.Animator.SetBool("IsWalking", false);
        aiMove.OnArrived -= Arrived;
    }


    private void Arrived()
    {
        stateManager.ChangeState(
            new CustomerOrderState(stateManager)
        );
    }
}
