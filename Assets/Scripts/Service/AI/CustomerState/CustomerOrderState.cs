using UnityEngine;

public class CustomerOrderState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    private bool receiveOrder;

    

    public CustomerOrderState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.Renderer.material.color = Color.orange;

        timer = 5.0f;
        receiveOrder = false;

        stateManager.CreateOrder();
        stateManager.OrderButton.SetOrder(stateManager.Order);
        Debug.Log("주문 시작");


        stateManager.OrderButton.gameObject.SetActive(true);

        stateManager.OrderButton.OnClicked += ReceiveOrder;

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
    }

    

    public void Exit()
    {
        Debug.Log("주문 종료");
        stateManager.OrderButton.OnClicked -= ReceiveOrder;
        stateManager.OrderButton.gameObject.SetActive(false);
    }

    private void ReceiveOrder()
    {
        receiveOrder = true;
        stateManager.OrderButton.gameObject.SetActive(false);
        stateManager.ChangeState(new CustomerWaitForFoodState(stateManager));
    }

    
}