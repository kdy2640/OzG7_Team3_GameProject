using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private OrderButton orderButton;
    public Renderer Renderer => gameObject.GetComponent<Renderer>();
    public AIMove AiMove => aiMove;
    public Transform ExitPoint => exitPoint;
    public OrderButton OrderButton => orderButton;

    private Table currentTable;
    private Transform seat;
    private DishAmount order;
    public Action foodReceived;
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
        IReadOnlyList<DishType> selectedDishes = GameManager.Instance.Market.MarketData.SelectedDishes;

        if (selectedDishes == null || selectedDishes.Count == 0)
        {
            Debug.Log("오늘의 메뉴가 없습니다.");
            return;
        }

        DishType dish = selectedDishes[UnityEngine.Random.Range(0, selectedDishes.Count)];
        order = new DishAmount(dish, UnityEngine.Random.Range(0,3));
    }
}
