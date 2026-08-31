using UnityEngine;

public class CustomerDrinkState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    private const float Duration = 3f;
    public CustomerDrinkState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.SetLifecycleProgress(0.85f);
        stateManager.AnimSetIdle();
        stateManager.Animator.SetBool("IsEating", true);
        if(!stateManager.DrinkZone.CanSpendDrink())
        {
            GoNext();
            return;
        }
        timer = Duration;
        //멘트 ON
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        stateManager.SetLifecycleProgress(
            Mathf.Lerp(0.85f, 0.87f, 1f - timer / Duration));



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
