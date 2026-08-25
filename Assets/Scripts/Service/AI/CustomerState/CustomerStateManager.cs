using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private float speed = 4f;
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private TipBox tipBox;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private OrderButton orderButton;
    [SerializeField] private Animator animator;
    [SerializeField] private DishRequestQueue requestQueue;
    [SerializeField] private WaitTimeBackGround waitBackGround;
    [SerializeField] private RunnerCatchButton runnerCatchButton;
    [SerializeField] private Dirty dirtyPrefab;
    [SerializeField] private float visibleCanvasHeight = 5.25f;
    [SerializeField] private Combo combo;

    private Table currentTable;
    private Transform seat;
    private DishAmount order;
    private Transform customerCanvas;
    public Action foodReceived;
    public Action caught;
    private float tipChance = 0.1f;
    private float eatTime = 5f;
    private float runChance = 0.5f;
    private float eatSpeedUpPercentage;
    public bool SeatDirty = false;
    

    public float Speed => speed;
    public AIMove AiMove => aiMove;
    public Transform ExitPoint => exitPoint;
    public TipBox TipBox => tipBox;
    public OrderButton OrderButton => orderButton;
    public Animator Animator => animator;
    public DishRequestQueue RequestQueue => requestQueue;   
    public WaitTimeBackGround WaitBackGround => waitBackGround;
    public RunnerCatchButton RunnerCatchButton => runnerCatchButton;
    public Dirty DirtyPrefab => dirtyPrefab;
    public Combo Combo => combo;
    
    public Table CurrentTable => currentTable;
    public Transform Seat => seat;
    public DishAmount Order => order;
    public float EatTime => eatTime;
    public float RunChance => runChance;
    
    [SerializeField]private IState currentState;

    private Action serviceEnd;

    #endregion

    #region State Machine Main
    public void Initialize(Transform exitPoint, TableManager tableManager, TipBox tipBox, DishRequestQueue queue, Dirty dirtyPrefab, Combo combo)
    {
        this.exitPoint = exitPoint;
        this.tableManager = tableManager;
        this.tipBox = tipBox;
        this.requestQueue = queue;
        this.dirtyPrefab = dirtyPrefab;
        this.combo = combo;
        AiMove.SetSpeed(speed);
    }

    private void OnEnable()
    {
        serviceEnd += CustomerDie;
        GameManager.Instance.Service.Events.Subscribe(ServiceEventType.LoopEnded, serviceEnd);
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

    private void LateUpdate()
    {
        bool isCanvasVisible =
            orderButton.gameObject.activeSelf
            || runnerCatchButton.gameObject.activeSelf;

        if (!isCanvasVisible)
            return;

        customerCanvas.position =
            transform.position + Vector3.up * visibleCanvasHeight;
    }


    public void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }

    #endregion

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

    
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
        animator.applyRootMotion = false;

        customerCanvas = orderButton.GetComponentInParent<Canvas>().transform;
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

    public void CreateDirty()
    {
        Vector3 dirtyPoint = transform.position + transform.forward * 1.0f + transform.up * 0.7f;
        Dirty dirty = Instantiate(DirtyPrefab, dirtyPoint, Quaternion.identity);
        dirty.SetCustomer(this);
        SeatDirty = true;
    }

    private void CustomerDie()
    {
            ChangeState(new CustomerGameOverState(this));
    }

    private void OnDisable()
    {
        serviceEnd -= CustomerDie;
        GameManager.Instance.Service.Events.Unsubscribe(ServiceEventType.LoopEnded ,serviceEnd);
        Destroy(this.gameObject);
    }

}
