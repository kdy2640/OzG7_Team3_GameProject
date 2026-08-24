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
        stateManager.EatSpeedApply();
        timer = stateManager.EatTime;
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
        stateManager.CurrentTable.ReleaseSeat(stateManager);

        if (Random.value < stateManager.RunChance)
        {
            stateManager.ChangeState(new CustomerRunState(stateManager));
            return;
        }

        DishDataSO data = DishDataDB.GetData(stateManager.Order.dish);

        if(data != null)
        {
            // 돈 획득
            GameManager.Instance.StockManager.AddCurrency(data.Cost);
            GameManager.Instance.Market.MarketData.TotalIncome += data.Cost;
            GameManager.Instance.Service.ResultBuilder.RecordDishSale(
                stateManager.Order.dish,
                data.Cost);
        }


        if (stateManager.IsTip())
        {
            stateManager.ChangeState(new CustomerGoToTipState(stateManager));
            return;
        }

        stateManager.ChangeState(new CustomerGoHomeState(stateManager));

    }

    public void Exit()
    {
        stateManager.Animator.SetBool("IsEating", false);
    }
}
