using System;
using UnityEngine;

public class ServerTakeMoneyFromRunnerState : IState
{
    private const float TakeMoneyTime = 2f;

    private ServerStateManager stateManager;
    private float timer;
    public ServerTakeMoneyFromRunnerState(ServerStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        timer = TakeMoneyTime * stateManager.WorkDurationMultiplier;
        // 돈받기 메시지 ON
        stateManager.ToastMessageOn(MessageType.sCatchRunner);
        stateManager.Animator.SetBool("IsAttacking", true);
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            stateManager.ChangeState(new ServerGetBackState(stateManager));
        }
    }

    public void Exit()
    {
        // 돈받기 메시지 OFF
        stateManager.Animator.SetBool("IsAttacking", false);
    }
}
