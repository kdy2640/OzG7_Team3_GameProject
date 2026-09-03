using Unity.VisualScripting;
using UnityEngine;

public class CustomerEatState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    private float duration;
    private float dirtyChance = 1f;
    public CustomerEatState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        stateManager.CreateDish();
        stateManager.Animator.SetBool("IsEating", true);
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_CustomerEat);
        stateManager.EatSpeedApply();
        duration = stateManager.EatTime;
        timer = duration;
        stateManager.SetLifecycleProgress(0.55f);
        if(stateManager.IsLateReceive)
        {
            stateManager.ToastMessageOn(MessageType.cLateReceive);
            return;
        }
        stateManager.ToastMessageOn(MessageType.cEat);
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        stateManager.SetLifecycleProgress(
            Mathf.Lerp(0.55f, 0.8f, 1f - timer / duration));

        if (timer <= 0)
        {
            FinishEating();
            return;
        }
    }

    

    private void FinishEating()
    {
        stateManager.SetLifecycleProgress(0.8f);
        stateManager.NotifyProcessingCompleted();

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
        stateManager.DestroyDish();
        stateManager.Animator.SetBool("IsEating", false);
    }
}
