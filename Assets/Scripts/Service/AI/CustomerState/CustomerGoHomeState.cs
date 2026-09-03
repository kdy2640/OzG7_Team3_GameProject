
using UnityEngine;

public class CustomerGoHomeState : IState
{

    private CustomerStateManager stateManager;
    private float timer;
    private bool arrived = false;
    public CustomerGoHomeState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.SetLifecycleProgress(0.9f);
        timer = 5.0f;

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
            TryDestroyCustomer();
            timer = 5.0f;
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
        arrived = true;
        stateManager.Combo.AddCount();
        stateManager.CompleteLifecycle();
    }
    private bool CheckSeatCleaned()
    {
        if (!stateManager.SeatDirty)
        {
            stateManager.CurrentTable.ReleaseSeat(stateManager);
            return true;
        }
        return false;
    }

    private void TryDestroyCustomer()
    {
        if(!arrived)
        {
            return;
        }
        if(CheckSeatCleaned())
        {
            stateManager.FinishLifecycle();
        }
    }
}
