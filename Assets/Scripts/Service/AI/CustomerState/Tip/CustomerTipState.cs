using UnityEngine;

public class CustomerTipState : IState
{
    private CustomerStateManager stateManager;
    private float timer;
    private const float Duration = 3f;

    public CustomerTipState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }
    public void Enter()
    {
        stateManager.SetLifecycleProgress(0.88f);
        stateManager.AnimSetIdle();
        stateManager.Animator.SetBool("IsTipping",true);
        stateManager.ToastMessageOn(MessageType.cTip);
        timer = Duration;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        stateManager.SetLifecycleProgress(
            Mathf.Lerp(0.88f, 0.9f, 1f - timer / Duration));

        if (timer <= 0)
        {
            Tip();
            return;
        }
    }
    public void Exit() 
    {
        stateManager.AiMove.OnArrived -= Tip;
        stateManager.Animator.SetBool("IsTipping", false);
    }

    private void Tip() 
    {
        DishDataSO data = DishDataDB.GetData(stateManager.Order.dish);
        stateManager.TipBox.AddTip(data.Cost / 5);

        stateManager.ChangeState(new CustomerGoHomeState(stateManager));
    }
}
