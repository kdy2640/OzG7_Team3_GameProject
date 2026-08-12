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
        timer = 3.0f;
        stateManager.GiveFood();
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
