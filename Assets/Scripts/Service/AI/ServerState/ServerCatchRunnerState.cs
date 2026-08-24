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
        stateManager.IsBusy = true;
        stateManager.AiMove.MoveTo(stateManager.Customer.transform);
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
        stateManager.Customer.caught?.Invoke();
        stateManager.ChangeState(new ServerTakeMoneyFromRunnerState(stateManager));
    }
}
