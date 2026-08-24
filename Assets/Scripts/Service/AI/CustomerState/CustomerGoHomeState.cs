
using UnityEngine;

public class CustomerGoHomeState : IState
{
    private CustomerStateManager stateManager;

    public CustomerGoHomeState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.CurrentTable.ReleaseSeat(stateManager);

        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.AiMove.OnArrived += ArrivedHome;

        stateManager.AiMove.SetSpeed(stateManager.Speed);

        stateManager.AiMove.MoveTo(stateManager.ExitPoint);
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
        GameObject.Destroy(stateManager.gameObject);
    }
}