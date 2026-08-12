
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
        stateManager.Animator.SetBool("IsTyping", true);
        stateManager.Renderer.material.color = Color.pink;
        stateManager.foodReceived += StartEat;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        stateManager.Animator.SetBool("IsTyping", false);
    }

    

    private void StartEat()
    {
        stateManager.ChangeState(
                new CustomerEatState(stateManager)
            );

        return;
    }
}