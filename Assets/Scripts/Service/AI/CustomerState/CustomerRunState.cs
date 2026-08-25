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
        stateManager.RunnerCatchButton.Initialize(stateManager);
        stateManager.RunnerCatchButton.gameObject.SetActive(true);
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
        if(stateManager.RunnerCatchButton != null )
        {
            stateManager.RunnerCatchButton.gameObject.SetActive(false);
        }
    }

    private void ArrivedHome()
    {
        stateManager.Combo.BreakCombo();
        GameObject.Destroy(stateManager.gameObject);
    }

    private void Caught()
    {
        stateManager.ChangeState(new CustomerCaughtState(stateManager));
        return;
    }
}
