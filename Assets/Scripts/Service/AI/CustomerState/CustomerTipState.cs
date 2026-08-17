using UnityEngine;

public class CustomerTipState : IState
{
    private CustomerStateManager stateManager;
    private float timer;

    public CustomerTipState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }
    public void Enter()
    {
        stateManager.AnimSetIdle();

        timer = 3.0f;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Tip();
            return;
        }
    }
    public void Exit() 
    {
        stateManager.AiMove.OnArrived -= Tip;
    }

    private void Tip() 
    {
        DishDataSO data = DishDataDB.GetData(stateManager.Order.dish);
        stateManager.TipBox.AddTip(data.Cost / 5);

        stateManager.ChangeState(new CustomerGoHomeState(stateManager));
    }
}
