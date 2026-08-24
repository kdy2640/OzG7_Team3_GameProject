using UnityEngine;

public class ServerMoveToCleanState : IState
{
    private ServerStateManager stateManager;
    private Dirty dirty;
    public ServerMoveToCleanState(ServerStateManager stateManager, Dirty dirty)
    {
        this.stateManager = stateManager;
        this.dirty = dirty;
    }

    public void Enter()
    {
        stateManager.AiMove.MoveTo(dirty.Customer.CurrentTable.GetServePoint(dirty.Customer.Seat));
        stateManager.AiMove.OnArrived += ArrivedDirty;
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        stateManager.AiMove.OnArrived -= ArrivedDirty;
    }

    private void ArrivedDirty()
    {
        stateManager.ChangeState(new ServerCleanState(stateManager, dirty));
    }
}