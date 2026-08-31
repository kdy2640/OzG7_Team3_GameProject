using UnityEngine;

public class CustomerDrinkState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    public CustomerDrinkState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.Animator.SetBool("IsEating", true);
        if(!stateManager.DrinkZone.CanSpendDrink())
        {
            GoNext();
            return;
        }
        timer = 3.0f;
        //멘트 ON
    }

    public void Execute()
    {
        timer -= Time.deltaTime;



        if (timer < 0)
        {
            stateManager.DrinkZone.SpendDrink();
            stateManager.PayDrink();
            GoNext();
        }
    }

    public void Exit()
    {
        stateManager.Animator.SetBool("IsEating", false);
    }

    private void GoNext()
    {
        if (stateManager.IsTip())
        {
            stateManager.ChangeState(new CustomerGoToTipState(stateManager));
            return;
        }

        stateManager.ChangeState(new CustomerGoHomeState(stateManager));
        return;
    }
}
