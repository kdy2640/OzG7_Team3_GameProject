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
        stateManager.Animator.SetBool("IsWalking", true);
        stateManager.AiMove.MoveTo(dirty.Customer.CurrentTable.GetServePoint(dirty.Customer.Seat));
        stateManager.AiMove.OnArrived += ArrivedDirty;
        stateManager.CleaningTool.SetActive(true);
        stateManager.ToastMessageOn(MessageType.sGoToClean);
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
        stateManager.Animator.SetBool("IsWalking", false);
        stateManager.ChangeState(new ServerCleanState(stateManager, dirty));

    }
}