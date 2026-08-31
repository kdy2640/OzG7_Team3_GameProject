using UnityEngine;

public class ServerSleepingState : IState
{
    private ServerStateManager stateManager;

    public ServerSleepingState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.IsBusy = true;
        stateManager.Animator.SetBool("IsSleeping", true);
        stateManager.AiMove.StopMove();
        stateManager.SleepingButton.gameObject.SetActive(true);
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        stateManager.SleepingButton.gameObject.SetActive(false);
        stateManager.Animator.SetBool("IsSleeping", false);
    }
}
