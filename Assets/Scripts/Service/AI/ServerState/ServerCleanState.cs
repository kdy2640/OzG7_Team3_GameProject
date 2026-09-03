
using UnityEngine;

public class ServerCleanState : IState
{
    private ServerStateManager stateManager;
    private Dirty dirty;
    private float cleaningTime = 1.0f;
    private float timer;

    

    public ServerCleanState(ServerStateManager stateManager,Dirty dirty)
    {
        this.stateManager = stateManager;
        this.dirty = dirty;
    }

    public void Enter()
    {
        stateManager.AiMove.SetDirection(dirty.Customer.Seat.position);
        stateManager.Animator.SetBool("IsAttacking", true);
        timer = cleaningTime * stateManager.WorkDurationMultiplier;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            timer = cleaningTime;
            if(dirty != null)
            {
                GameObject.Destroy(dirty.gameObject);
            }
            
            dirty.Customer.SeatDirty = false;
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_NegativeEventResolve);

            stateManager.isAutoWorking = false;
            stateManager.ChangeState(new ServerGetBackState(stateManager));
            return;
        }
    }

    public void Exit()
    {
        stateManager.Animator.SetBool("IsAttacking", false);
        stateManager.CleaningTool.SetActive(false);
    }
}
