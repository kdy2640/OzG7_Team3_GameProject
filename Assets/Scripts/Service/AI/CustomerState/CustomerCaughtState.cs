using UnityEngine;

public class CustomerCaughtState : IState
{
    private float timer = float.MaxValue;
    private CustomerStateManager stateManager;
    public CustomerCaughtState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.AnimSetIdle();
        stateManager.RunnerCatchButton.gameObject.SetActive(false);
        // 사과 메시지 ON
        stateManager.caught += Pay;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            stateManager.ChangeState(new CustomerGoHomeState(stateManager));
        }
    }

    public void Exit()
    {
       // 사과 메시지 OFF
       stateManager.caught -= Pay;
    }

    private void Pay()
    {
        timer = 2f;
        DishDataSO data = DishDataDB.GetData(stateManager.Order.dish);

        if (data != null)
        {
            // 돈 획득
            GameManager.Instance.StockManager.AddCurrency(data.Cost);
            GameManager.Instance.Market.MarketData.TotalIncome += data.Cost;


            int bonusCurrency = (int)(data.Cost * stateManager.Combo.BonusRate / 100);
            GameManager.Instance.StockManager.AddCurrency(bonusCurrency);
            GameManager.Instance.Market.MarketData.TotalIncome += bonusCurrency;

            GameManager.Instance.Service.ResultBuilder.RecordDishSale(
                stateManager.Order.dish,
                data.Cost + bonusCurrency);
        }
    }
}
