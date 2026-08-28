using UnityEngine;

public class ServerCatchRunnerState : IState
{
    private ServerStateManager stateManager;

    public ServerCatchRunnerState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.Animator.SetBool("IsRunning", true);
        stateManager.IsBusy = true;
        stateManager.AiMove.MoveToNear(stateManager.Customer.transform);
        stateManager.AiMove.OnArrived += Catch;
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        stateManager.AiMove.OnArrived -= Catch;
    }

    private void Catch()
    {
        stateManager.AiMove.SetDirection(stateManager.Customer.transform.position);
        stateManager.Animator.SetBool("IsRunning", false);
        stateManager.Customer.caught?.Invoke();
        stateManager.ChangeState(new ServerTakeMoneyFromRunnerState(stateManager));
    }
}
