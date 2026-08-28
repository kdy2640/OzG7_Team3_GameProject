using System.Collections;
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
        stateManager.AiMove.SetDirection(stateManager.CurrentTable.transform.position);
        stateManager.Animator.SetBool("IsOrdering", true);

        timer = 10.0f;

        stateManager.OrderButton.gameObject.SetActive(true);
        stateManager.CreateOrder();
        stateManager.OrderButton.SetOrder(stateManager.Order);
        stateManager.RequestQueue.Queue.Enqueue(stateManager.Order.dish);
        Debug.Log("주문 시작");
        stateManager.OrderButton.OnClicked += ReceiveOrder;
        stateManager.WaitBackGround.SetWaitTimeUI(timer);
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        stateManager.WaitBackGround.RunWaitTimeUI(timer);

        if (timer <= 0)
        {
            if (stateManager.RequestQueue.Queue.Count > 0)
            {
                stateManager.RequestQueue.Queue.Dequeue();
            }
            stateManager.ChangeState(
                new CustomerAngryGoState(stateManager)
            );

            return;
        }
    }

    

    public void Exit()
    {
        stateManager.Animator.SetBool("IsOrdering", false);
        stateManager.OrderButton.OnClicked -= ReceiveOrder;
    }

    private void ReceiveOrder()
    {
        if(stateManager.RequestQueue.Queue.Count > 0)
        {
            stateManager.RequestQueue.Queue.Dequeue();
        }
        stateManager.DeactiveOrderButton();
        stateManager.ChangeState(new CustomerWaitForFoodState(stateManager));
    }

    
}