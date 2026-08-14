using UnityEngine;

public class ServerGetBackState : IState
{
    private ServerStateManager stateManager;


    public ServerGetBackState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.IsBusy = false;

        stateManager.AiMove.OnArrived += ArrivedHome;

        stateManager.AiMove.MoveTo(
            stateManager.WaitPoint
        );
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        stateManager.Animator.SetBool("IsWalking", false);
        stateManager.AiMove.OnArrived -= ArrivedHome;
    }


    private void ArrivedHome()
    {
        stateManager.ChangeState(
            new ServerIdleState(stateManager)
        );
    }
}