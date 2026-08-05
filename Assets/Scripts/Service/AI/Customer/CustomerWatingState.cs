using UnityEngine;

public class CustomerWatingState : IState
{
    private CustomerStateManager stateManager;
    private TableManager tableManager;
    private Table table;

    public CustomerWatingState(CustomerStateManager stateManager,TableManager tableManager)
    {
        this.tableManager = tableManager;
        this.stateManager = stateManager;
    }

    public void Enter()
    {
        stateManager.AiMove.StopMove();
        tableManager.AddWaitingCustomer(stateManager);
        Debug.Log("웨이팅 시작");
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        
    }
}
