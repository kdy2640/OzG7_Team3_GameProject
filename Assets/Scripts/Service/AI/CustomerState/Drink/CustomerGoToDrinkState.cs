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
        stateManager.AiMove.MoveTo(stateManager.DrinkZone.transform);
        stateManager.AiMove.OnArrived += ArrivedDrink;
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }

    private void ArrivedDrink()
    {
        stateManager.ChangeState(new CustomerDrinkState(stateManager));
        return;
    }
}
