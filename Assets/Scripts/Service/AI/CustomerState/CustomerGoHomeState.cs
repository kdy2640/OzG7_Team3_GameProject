
using UnityEngine;

public class CustomerGoHomeState : IState
{

    private CustomerStateManager stateManager;
    private float timer;
    public CustomerGoHomeState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {

        stateManager.Animator.SetBool("IsWalking", true);

        stateManager.AiMove.OnArrived += ArrivedHome;

        stateManager.AiMove.SetSpeed(stateManager.Speed);

        stateManager.AiMove.MoveTo(stateManager.ExitPoint);
    }


    public void Execute()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            CheckSeatCleaned();
            timer = 2.0f;
        }
    }


    public void Exit()
    {
        if (stateManager != null)
        {
            stateManager.Animator.SetBool("IsWalking", false);
        }
        stateManager.AiMove.OnArrived -= ArrivedHome;
    }

    private void ArrivedHome()
    {
        // 오늘의 손님 ++;
        // 콤보 ++;
        stateManager.Combo.AddCount();
        CheckSeatCleaned();
    }
    private void CheckSeatCleaned()
    {
        if (!stateManager.SeatDirty)
        {
            stateManager.CurrentTable.ReleaseSeat(stateManager);
            GameObject.Destroy(stateManager.gameObject);
        }
    }
}