using UnityEngine;

public class CustomerOrderState : IState
{
    private CustomerStateManager stateManager;
    private float timer;

    

    public CustomerOrderState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.AiMove.SetDirection(
            (stateManager.CurrentTable.transform.position - stateManager.transform.position)
            .normalized
            );
        stateManager.Animator.SetBool("IsTyping", true);
        stateManager.Renderer.material.color = Color.orange;

        timer = 5.0f;

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
        stateManager.Animator.SetBool("IsTyping", false);
        stateManager.OrderButton.OnClicked -= ReceiveOrder;
        stateManager.OrderButton.gameObject.SetActive(false);
    }

    private void ReceiveOrder()
    {
        stateManager.OrderButton.gameObject.SetActive(false);
        stateManager.ChangeState(new CustomerWaitForFoodState(stateManager));
    }

    
}