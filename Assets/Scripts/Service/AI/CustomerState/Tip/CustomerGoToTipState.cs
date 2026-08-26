using UnityEngine;

public class CustomerGoToTipState : IState
{
    private CustomerStateManager stateManager;
    public CustomerGoToTipState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.AiMove.OnArrived += StartTip;

        stateManager.AiMove.MoveTo(stateManager.TipBox.transform);
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }

    private void StartTip()
    {
        stateManager.ChangeState(new CustomerTipState(stateManager));
    }
}
