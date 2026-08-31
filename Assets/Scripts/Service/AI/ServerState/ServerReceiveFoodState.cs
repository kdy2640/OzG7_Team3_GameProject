using UnityEngine;

public class ServerReceiveFoodState : IState
{
    private ServerStateManager stateManager;
    public ServerReceiveFoodState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    private float timer;

    public void Enter()
    {
        timer = stateManager.ReceiveFoodTime;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_DishPickup);
            stateManager.ChangeState(new ServerMoveToTableState(stateManager, stateManager.AiMove, stateManager.ServePoint));
            return;
        }
    }

    public void Exit()
    {
        stateManager.CreateDish();
    }
}
