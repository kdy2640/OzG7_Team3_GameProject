using Unity.VisualScripting;
using UnityEngine;

public class CustomerGoHomeState : IState
{
    private CustomerStateManager stateManager;
    private AIMove aiMove;
    private Transform exitPoint;
    private bool isTipable = false;

    public CustomerGoHomeState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.CurrentTable.ReleaseSeat(stateManager);

        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.AiMove.OnArrived += ArrivedHome;

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