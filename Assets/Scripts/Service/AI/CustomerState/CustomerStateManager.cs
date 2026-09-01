using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerStateManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private float speed = 8f;
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
    [SerializeField] private DrinkZone drinkZone;
    [SerializeField] private Image autoServeImg;

    private Table currentTable;
    private Transform seat;
    private DishAmount order;
    private Transform customerCanvas;
    public Action foodReceived;
    public Action caught;
    public event Action ProcessingCompleted;
    public event Action<CustomerStateManager> LifecycleFinished;
    private float tipChance = 0.1f;
    private float eatTime = 5f;
    private float runChance = 0.1f;
    private float eatSpeedUpPercentage;
    private bool isProcessingCompleted;
    private bool isLifecycleFinished;
    [SerializeField, Range(0f, 1f)] private float lifecycleProgress;
    public bool SeatDirty = false;
    public bool IsAutoServed = false;
    private GameObject dishObject;
    private Action serviceLoopEnd;
    
    

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
    public DrinkZone DrinkZone => drinkZone;
    
    public Table CurrentTable => currentTable;
    public Transform Seat => seat;
    public DishAmount Order => order;
    public float EatTime => eatTime;
    public float RunChance => runChance;
    public float LifecycleProgress => lifecycleProgress;
    
    [SerializeField]private IState currentState;

    #endregion

    #region State Machine Main
    public void Initialize(Transform exitPoint, TableManager tableManager, TipBox tipBox, DishRequestQueue queue, Dirty dirtyPrefab, Combo combo, DrinkZone drinkZone)
    {
        this.exitPoint = exitPoint;
        this.tableManager = tableManager;
        this.tipBox = tipBox;
        this.requestQueue = queue;
        this.dirtyPrefab = dirtyPrefab;
        this.combo = combo;
        this.drinkZone = drinkZone;
        lifecycleProgress = 0f;
        AiMove.SetSpeed(speed);
    }

    private void Start()
    {
        if (tableManager.IsThereAnyWaiting())
        {
            ChangeState(new CustomerWatingState(this, tableManager));
            return;
        }

        else
        {
            currentTable = tableManager.FindEmptyTable();
            Debug.Log(this + " : " + CurrentTable);

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

    private void OnEnable()
    {
        serviceLoopEnd += Die;
        GameManager.Instance.Service.Events.Subscribe(ServiceEventType.LoopEnded, serviceLoopEnd);
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

    

    public void Pay()  // 돈 획득 이펙트
    {
        DishDataSO data = DishDataDB.GetData(Order.dish);
        int basicPrice = DishPriceCalculator.BasicPriceCalculate(data.Dish);
        if (data != null)
        {
            GameManager.Instance.StockManager.AddCurrency(basicPrice);
            GameManager.Instance.Market.MarketData.TotalIncome += basicPrice;

            int bonusCurrency = (int)(basicPrice * Combo.BonusRate / 100);

            GameManager.Instance.StockManager.AddCurrency(bonusCurrency);
            GameManager.Instance.Market.MarketData.TotalIncome += bonusCurrency;

            GameManager.Instance.Service.ResultBuilder.RecordDishSale(
                Order.dish,
                basicPrice + bonusCurrency);

            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_CustomerPay);
        }
    }

    public void PayDrink()
    {
        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(FacilityType.Decor_2);
        int drinkPrice = -100 + level * 300;
        GameManager.Instance.StockManager.AddCurrency(drinkPrice);
        GameManager.Instance.Market.MarketData.TotalIncome += drinkPrice;
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_DrinkServed);
    }

    public void NotifyProcessingCompleted()
    {
        if (isProcessingCompleted)
            return;

        isProcessingCompleted = true;
        ProcessingCompleted?.Invoke();
    }

    public void SetLifecycleProgress(float progress)
    {
        lifecycleProgress = Mathf.Max(
            lifecycleProgress,
            Mathf.Clamp01(progress));
    }

    public void CompleteLifecycle()
    {
        if (isLifecycleFinished)
            return;

        isLifecycleFinished = true;
        SetLifecycleProgress(1f);
        LifecycleFinished?.Invoke(this);
    }

    public void FinishLifecycle()
    {
        CompleteLifecycle();
        Destroy(gameObject);
    }

    public void DeactiveOrderButton()
    {
        animator.SetBool("IsSitting", true);
        StartCoroutine(DeactiveOrderButtonCo());
    }

    private IEnumerator DeactiveOrderButtonCo()
    {
        OrderButton.gameObject.SetActive(false);
        if (IsAutoServed)
        {
            autoServeImg.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            IsAutoServed = false;
            autoServeImg.gameObject.SetActive(false);
        }
    }

    public void CreateDirty()
    {
        Dirty dirty = Instantiate(DirtyPrefab, currentTable.GetFoodSpot(this).position + Vector3.up, Quaternion.identity);
        dirty.SetCustomer(this);
        SeatDirty = true;
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_NegativeEventStart);
    }

    public void CreateDish()
    {
        dishObject = Instantiate(DishDataDB.GetData(Order.dish).DishPrefab, currentTable.GetFoodSpot(this).position + Vector3.up * 2f, Quaternion.identity);
    }

    public void DestroyDish()
    {
        Destroy(dishObject);
    }

    private void Die()
    {
        ChangeState(new CustomerGameOverState(this));
    }

    private void OnDisable()
    {
        serviceLoopEnd -= Die;
        GameManager.Instance.Service.Events.Unsubscribe(ServiceEventType.LoopEnded, serviceLoopEnd);
    }
}
