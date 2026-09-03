using UnityEngine;

public class CustomerGoToDrinkState : IState
{
    private CustomerStateManager stateManager;
    public CustomerGoToDrinkState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.SetLifecycleProgress(0.82f);
        stateManager.Animator.SetBool("IsWalking", true);
        stateManager.AiMove.MoveTo(stateManager.DrinkZone.DrinkSpot);
        
        stateManager.AiMove.OnArrived += ArrivedDrink;
        stateManager.ToastMessageOn(MessageType.cGoToDrink);
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }

    private void ArrivedDrink()
    {
        stateManager.AiMove.OnArrived -= ArrivedDrink;
        stateManager.AiMove.SetDirection(stateManager.DrinkZone.transform.position);
        stateManager.Animator.SetBool("IsWalking", false);
        stateManager.ChangeState(new CustomerDrinkState(stateManager));
        return;
    }
}
