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
        if(GameManager.Instance.Upgrade.RuntimeLevel.Get(FacilityType.Decor_6) < 1)
        {
            stateManager.ChangeState(new CustomerGoHomeState(stateManager));
        }

        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.AiMove.MoveTo(stateManager.TipBox.TipSpot);

        stateManager.AiMove.OnArrived += StartTip;
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }

    private void StartTip()
    {
        stateManager.AiMove.OnArrived -= StartTip;
        stateManager.AiMove.SetDirection(stateManager.TipBox.transform.position);
        stateManager.Animator.SetBool("IsWalking", false);
        stateManager.ChangeState(new CustomerTipState(stateManager));
        return;
    }
}
