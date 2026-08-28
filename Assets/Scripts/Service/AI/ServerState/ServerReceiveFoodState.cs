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
            stateManager.ChangeState(new ServerMoveToTableState(stateManager, stateManager.AiMove, stateManager.ServePoint));
            return;
        }
    }

    public void Exit()
    {
        //음식 프리팹 생성
    }
}
