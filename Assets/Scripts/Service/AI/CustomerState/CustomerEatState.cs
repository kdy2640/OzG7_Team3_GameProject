using Unity.VisualScripting;
using UnityEngine;

public class CustomerEatState : IState
{
    

    private CustomerStateManager stateManager;

    private float timer;

    public CustomerEatState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        stateManager.Animator.SetBool("IsEating", true);
        stateManager.Renderer.material.color = Color.yellow;
        timer = 10.0f;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FinishEating();
            return;
        }
    }

    private void FinishEating()
    {
        stateManager.Animator.SetBool("IsEating", false);

        DishDataSO data = DishDataDB.GetData(stateManager.Order.dish);

        if(data != null)
        {
            // 돈 획득
            GameManager.Instance.StockManager.AddCurrency(data.Cost);
        }


        stateManager.ChangeState(new CustomerGoHomeState(stateManager));
    }

    public void Exit()
    {
        
    }
}