using UnityEngine;

public class CustomerAngryGoState : IState
{
    private CustomerStateManager stateManager;

    public CustomerAngryGoState(CustomerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.SetLifecycleProgress(0.9f);
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_CustomerAngry);
        stateManager.Combo.BreakCombo();

        stateManager.OrderButton.gameObject.SetActive(false);

        stateManager.CurrentTable.ReleaseSeat(stateManager);

        stateManager.Animator.SetBool("IsAngryWalking", true);

        stateManager.AiMove.OnArrived += ArrivedHome;

        stateManager.AiMove.MoveTo(stateManager.ExitPoint);
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        if (stateManager != null)
        {
            stateManager.Animator.SetBool("IsAngryWalking", false);
        }
        stateManager.AiMove.OnArrived -= ArrivedHome;
    }

    private void ArrivedHome()
    {
        stateManager.FinishLifecycle();
    }
}
