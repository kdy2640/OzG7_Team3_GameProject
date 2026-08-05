using UnityEngine;

public class CustomerOrderState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    private bool receiveFood;

    

    public CustomerOrderState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        timer = 5.0f;
        receiveFood = false;

        stateManager.CreateOrder();
        stateManager.OrderButton.SetOrder(stateManager.Order);
        Debug.Log("주문 시작");


        stateManager.OrderButton.gameObject.SetActive(true);

        stateManager.OrderButton.OnClicked += ReceiveFood;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            stateManager.ChangeState(
                new CustomerGoHomeState(stateManager)
            );

            return;
        }
        if (receiveFood)
        {
            stateManager.ChangeState(
                new CustomerEatState(stateManager)
            );

            return;
        }
       
    }

    private void ReceiveFood()
    {
        receiveFood = true; 
    }

    public void Exit()
    {
        Debug.Log("주문 종료");
        stateManager.OrderButton.OnClicked -= ReceiveFood;
        stateManager.OrderButton.gameObject.SetActive(false);
    }
}