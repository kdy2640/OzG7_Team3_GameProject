using UnityEngine;

public class CustomerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform exitPoint;
    
    [SerializeField] private TableManager tableManager;
    public AIMove AiMove => aiMove;
    public Transform ExitPoint => exitPoint;

    private Table currentTable;
    private Transform seat;

    public Table CurrentTable => currentTable;
    public Transform Seat => seat;
    private IState currentState;

    private void Awake()
    {
        if(exitPoint == null)
        {
            exitPoint = FindFirstObjectByType<CustomerSpawner>().transform;
        }
        if(tableManager  == null)
        {
            tableManager = FindFirstObjectByType<TableManager>();
        }
    }

    private void Start()
    {
        currentTable = tableManager.FindEmptyTable();

        if (tableManager.IsThereAnyWaiting())
        {
            ChangeState(new CustomerWatingState(this, tableManager));
            return;
        }

        else
        {
            if (currentTable == null)
            {
                ChangeState(new CustomerWatingState(this, tableManager));
                return;
            }
            else
            {
                seat = currentTable.ReserveSeat(this);
                AssignTable(currentTable, seat);
                ChangeState(new CustomerMoveToTableState(this, aiMove, seat));
            }
        }

        
    }

    private void Update()
    {
        currentState?.Execute();
    }


    public void ChangeState(IState newState)
    {
        Debug.Log("상태변경 : " + currentState + " => " + newState);

        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }

    public void AssignTable(Table table, Transform seat)
    {
        currentTable = table;
        this.seat = seat;
    }
}
