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
        stateManager.animator.SetBool("IsWalking", true);

        stateManager.IsBusy = false;

        stateManager.Renderer.material.color = Color.black;

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
        stateManager.animator.SetBool("IsWalking", false);
        stateManager.AiMove.OnArrived -= ArrivedHome;
    }


    private void ArrivedHome()
    {
        stateManager.ChangeState(
            new ServerIdleState(stateManager)
        );
    }
}