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
        timer = 5.0f;
        Debug.Log("음식 제공 중");
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
        Debug.Log("제공 완료");
    }
}
