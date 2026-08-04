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
        timer = 5.0f;
        Debug.Log("식사 시작");
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
        Debug.Log("식사 종료");
    }
}