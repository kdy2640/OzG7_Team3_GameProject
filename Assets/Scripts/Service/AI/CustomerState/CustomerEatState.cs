using Unity.VisualScripting;
using UnityEngine;

public class CustomerEatState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    private float dirtyChance = 1f;
    public CustomerEatState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        stateManager.Animator.SetBool("IsEating", true);
        stateManager.EatSpeedApply();
        timer = stateManager.EatTime;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FinishEating();
            return;
        }
    }

    

    private void FinishEating()
    {
        if(Random.value < dirtyChance)
        {
            stateManager.CreateDirty();
        }

        if(!stateManager.SeatDirty)
        {
            stateManager.CurrentTable.ReleaseSeat(stateManager);
        }
        

        if (Random.value < stateManager.RunChance)
        {
            stateManager.ChangeState(new CustomerRunState(stateManager));
            return;
        }

        stateManager.Pay();

        if(stateManager.DrinkZone.CanSpendDrink()&&stateManager.DrinkZone != null)
        {
            stateManager.ChangeState(new CustomerGoToDrinkState(stateManager));
            return;
        }

        if (stateManager.IsTip())
        {
            stateManager.ChangeState(new CustomerGoToTipState(stateManager));
            return;
        }

        stateManager.ChangeState(new CustomerGoHomeState(stateManager));

    }

    public void Exit()
    {
        stateManager.Animator.SetBool("IsEating", false);
    }
}
