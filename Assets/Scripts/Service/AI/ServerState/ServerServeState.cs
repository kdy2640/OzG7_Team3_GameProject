using UnityEngine;

public class ServerServeState : IState
{
    private ServerStateManager stateManager;
    private float timer;

    public ServerServeState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.AiMove.SetDirection(
                (
                    stateManager.Customer.CurrentTable.transform.position
                )
            );
        stateManager.AnimSetIdle();
        timer = stateManager.ServeTime;
        stateManager.GiveFood();
        stateManager.DestroyDish();
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_DishServed);
        if(stateManager.Customer.IsLateReceive)
        {
            stateManager.ToastMessageOn(MessageType.sLateServe);
            return;
        }
        stateManager.ToastMessageOn(MessageType.sServe);
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            stateManager.ChangeState(
                new ServerGetBackState(stateManager)
            );

            return;
        }
    }

    public void Exit()
    {
        
    }
}
