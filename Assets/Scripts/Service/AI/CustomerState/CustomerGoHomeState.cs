using UnityEngine;

public class CustomerGoHomeState : IState
{
    private CustomerStateManager stateManager;
    private AIMove aiMove;
    private Transform exitPoint;


    public CustomerGoHomeState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.CurrentTable.ReleaseSeat(stateManager);

        stateManager.AiMove.OnArrived += ArrivedHome;
        
        stateManager.AiMove.MoveTo(stateManager.ExitPoint);

        stateManager.Renderer.material.color = Color.green;
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