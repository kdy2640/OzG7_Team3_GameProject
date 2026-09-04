
using UnityEngine;

public class CustomerWaitForFoodState : IState
{
    private CustomerStateManager stateManager;



    public CustomerWaitForFoodState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.SetLifecycleProgress(0.4f);
        stateManager.Animator.SetBool("IsSitting", true);
        stateManager.foodReceived += StartEat;
        if(!stateManager.IsAutoServed)
            stateManager.ToastMessageOn(MessageType.cHungry);
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        stateManager.Animator.SetBool("IsSitting", false);
    }

    

    private void StartEat()
    {
        stateManager.ChangeState(
                new CustomerEatState(stateManager)
            );

        return;
    }
}
