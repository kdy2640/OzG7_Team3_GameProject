using UnityEngine;

public class CustomerRunState : IState
{
    private CustomerStateManager stateManager;

    public CustomerRunState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.Animator.SetBool("IsWalking", true);
        stateManager.AiMove.OnArrived += ArrivedHome;
        stateManager.caught += Caught;
        stateManager.RunnerCatchButton.gameObject.SetActive(true);
        stateManager.RunnerCatchButton.Initialize(stateManager);
        stateManager.AiMove.SetSpeed(0.5f);

        stateManager.AiMove.MoveTo(stateManager.ExitPoint);
    }

    public void Execute()
    {
        
        
    }

    public void Exit()
    {
        stateManager.AiMove.OnArrived -= ArrivedHome;
        stateManager.caught -= Caught;
        
    }

    private void ArrivedHome()
    {
        GameObject.Destroy(stateManager.gameObject);
    }

    private void Caught()
    {
        stateManager.ChangeState(new CustomerCaughtState(stateManager));
        return;
    }
}
