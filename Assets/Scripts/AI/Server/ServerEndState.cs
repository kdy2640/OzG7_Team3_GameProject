using UnityEngine;

public class ServerEndState : IState
{
    private ServerStateManager stateManager;

    public ServerEndState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    public void Enter()
    {
        Debug.Log("손님 퇴장 완료");
    }


    public void Execute()
    {

    }


    public void Exit()
    {

    }
}