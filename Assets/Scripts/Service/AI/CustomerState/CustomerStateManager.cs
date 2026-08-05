using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private OrderButton orderButton;

    public AIMove AiMove => aiMove;
    public Transform ExitPoint => exitPoint;
    public OrderButton OrderButton => orderButton;

    private Table currentTable;
    private Transform seat;
    private DishAmount order;
    public Table CurrentTable => currentTable;
    public Transform Seat => seat;
    public DishAmount Order => order;
    

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
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }

    public void AssignTable(Table table, Transform seat)
    {
        currentTable = table;
        this.seat = seat;
    }

    public void CreateOrder()
    {
        IReadOnlyList<DishType> selectedDishes = GameManager.Instance.Market.Data.SelectedDishes;

        if (selectedDishes == null || selectedDishes.Count == 0)
        {
            Debug.Log("선택된 메뉴가 없습니다.");
            return;
        }

        DishType dish = selectedDishes[Random.Range(0, selectedDishes.Count)];
        order = new DishAmount(dish, 1);
    }
}
