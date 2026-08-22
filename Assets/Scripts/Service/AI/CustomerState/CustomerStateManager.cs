using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateManager : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private TipBox tipBox;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private OrderButton orderButton;
    [SerializeField] private Animator animator;
    [SerializeField] private DishRequestQueue requestQueue;
    [SerializeField] private WaitTimeBackGround waitBackGround;

    private Table currentTable;
    private Transform seat;
    private DishAmount order;
    public Action foodReceived;
    private float tipChance = 0.1f;
    private float eatTime = 5f;
    

    public AIMove AiMove => aiMove;
    public Transform ExitPoint => exitPoint;
    public TipBox TipBox => tipBox;
    public OrderButton OrderButton => orderButton;
    public Animator Animator => animator;
    public DishRequestQueue RequestQueue => requestQueue;   
    public WaitTimeBackGround WaitBackGround => waitBackGround;
    
    public Table CurrentTable => currentTable;
    public Transform Seat => seat;
    public DishAmount Order => order;
    public float EatTime => eatTime;

    private float eatSpeedUpPercentage;

    [SerializeField]private IState currentState;


    public void Initialize(Transform exitPoint, TableManager tableManager, TipBox tipBox, DishRequestQueue queue)
    {
        this.exitPoint = exitPoint;
        this.tableManager = tableManager;
        this.tipBox = tipBox;
        this.requestQueue = queue;
        AiMove.SetSpeed(speed);
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
        order = new DishAmount(dish, 1);
    }

    public void AnimSetIdle()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
        animator.applyRootMotion = false;
    }

    public bool IsTip()
    {
        return UnityEngine.Random.value < tipChance;
    }

    public void EatSpeedUp(float percentage)
    {
        eatSpeedUpPercentage += percentage;
    }

    public void EatSpeedApply()
    {
        eatTime = eatTime / (1 + (eatSpeedUpPercentage / 100));
    }

    public void TipChanceUp()
    {
        tipChance *= 2;
    }
}
