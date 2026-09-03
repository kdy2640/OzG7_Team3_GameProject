using UnityEngine;

public class CustomerCaughtState : IState
{
    private float timer;
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
        Pay();
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
    }

    private void Pay()
    {
        timer = 2f;
        stateManager.Pay();
    }
}
