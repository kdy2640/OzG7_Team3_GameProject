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
        stateManager.IsBusy = true;
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_NegativeEventStart);
        // 자는 애니메이션 시작
        stateManager.AiMove.StopMove();
        stateManager.SleepingButton.gameObject.SetActive(true);
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        stateManager.SleepingButton.gameObject.SetActive(false);
        // 자는 애니메이션 종료
    }
}
