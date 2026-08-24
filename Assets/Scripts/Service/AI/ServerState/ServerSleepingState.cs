using UnityEngine;

public class ServerSleepingState : IState
{
    private ServerStateManager stateManager;

    public ServerSleepingState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        // 자는 애니메이션 시작
        stateManager.AiMove.StopMove();

    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        // 자는 애니메이션 종료
    }
}
