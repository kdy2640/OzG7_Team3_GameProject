using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ServerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform servePoint;
    [SerializeField] private Transform kitchen;
    [SerializeField] private Transform waitPoint;
    [SerializeField] private Transform foodSpot;
    [SerializeField] private GameObject tray;
    [SerializeField] private SleepingButton sleepingButton;
    [SerializeField] private Image AutoWorkingImg;
    [SerializeField] private GameObject cleaningTool;
    [SerializeField] private ToastMessage toastMessage;


    [SerializeField] private float baseSpeed = 8;
    [SerializeField] private float speed;
    [SerializeField] private int level;
    [SerializeField] private float serveTime = 3f;
    [SerializeField] private float receiveFoodTime = 3f;
   
    private Animator animator;

    private DishType dish;

    private CustomerStateManager customer;

    public bool IsBusy = false;

    public event Action customerChanged;

    private DishEffectQueue dishEffectQueue;

    private float sleepingChance = 0.05f;

    private GameObject dishPrefab;
    private GameObject dishObject;
    
    public float Speed => speed;
    public AIMove AiMove => aiMove;
    public Transform ServePoint => servePoint;
    public Transform Kitchen => kitchen;
    public Transform WaitPoint => waitPoint;
    public Transform FoodSpot => foodSpot;
    public Animator Animator => animator;
    public CustomerStateManager Customer => customer;
    public DishType Dish => dish;
    public int Level => level; 
    public float ServeTime => serveTime;
    public float ReceiveFoodTime => receiveFoodTime;
    public SleepingButton SleepingButton => sleepingButton;
    public GameObject DishPrefab => dishPrefab;
    public GameObject CleaningTool => cleaningTool;


    public bool isAutoWorking = false;

    [SerializeField] private IState currentState;
    private Action serviceEnd;

    public void Initialize()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        animator.applyRootMotion = false;
    }

    private void Awake()
    {
        kitchen = FindFirstObjectByType<Kitchen>().transform;
        aiMove = gameObject.GetComponent<AIMove>();
        
        dishEffectQueue = FindFirstObjectByType<DishEffectQueue>();
        speed = baseSpeed;
        UpdateStatus();
    }

    private void OnEnable()
    {
        tray.gameObject.SetActive(false);
        serviceEnd += ServerDie;
        GameManager.Instance.Service.Events.Subscribe(ServiceEventType.LoopEnded, serviceEnd);
    }

    private void Start()
    {
        ChangeState(new ServerGetBackState(this));
        StartCoroutine(SleepingChanceCo());
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

    public void SetServerSpot(Transform spot)
    {
        waitPoint = spot;
    }

    public void SetServerDish(DishType dish, CustomerStateManager customer)
    {
        this.customer = customer;
        customerChanged?.Invoke();

        this.dish = dish;

        servePoint = customer.CurrentTable.GetServePoint(customer.Seat);


        CookEffectApply();

        ChangeState(new ServerMoveToKitchenState(this, aiMove, kitchen));
    }

    public void GiveFood()
    {
        if( isAutoWorking)
        {
            customer.IsAutoServed = true;
        }
        customer.foodReceived?.Invoke();
        
        isAutoWorking = false;
    }

    public void AnimSetIdle()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsServing", false);
        animator.SetBool("IsRunning", false);
    }

    public void SetLevel(EmployeeType employee)
    {
        level = GameManager.Instance.Upgrade.RuntimeLevel.Get(employee);
    }
    #region 스킬 관련
    public void UpdateStatus()
    {
        speed = baseSpeed;
        for (int i = 0; i < level - 1; i++)
        {
            UpgradeSpeed();
        }
        aiMove.SetSpeed(speed);
    }

    private void UpgradeSpeed()
    {
        speed *= 1.1f;
    }
    public void UpgradeBaseSpeed()
    {
        baseSpeed *= 2;
        UpdateStatus();
    }
    public void WorkSpeedUp()
    {
        serveTime /= 2;
        receiveFoodTime /= 2;
    }

    public void CustomerEatSpeedUp()
    {
        customer.EatSpeedUp(50.0f);
    }

    public void CookerCustomerEatSpeedUp()
    {
        customer.EatSpeedUp(30.0f);
    }

    public void CustomerTipChanceUp()
    {
        customer.TipChanceUp();
    }

    private IEnumerator AutoServeCo()
    {
        while(true)
        {
            if (!IsBusy)
            {
                OrderButton orderButton = FindFirstObjectByType<OrderButton>();
                if (orderButton != null)
                {
                    customer = orderButton.Customer;
                    customer.IsAutoServed = true;
                    isAutoWorking = true;
                    orderButton.OnClick();
                }

                if(!IsBusy)
                {
                    CleaningButton cleaningButton = FindFirstObjectByType<CleaningButton>();
                    if(cleaningButton != null)
                    {
                        cleaningButton.OnClick();
                        isAutoWorking = true;
                    }
                }
                
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void AutoServe()
    {
        SetAutoWorkingUI();
        StartCoroutine(AutoServeCo());
    }

    #endregion

    private void CookEffectApply()
    {

        if(dishEffectQueue.EatSpeedUpQueue.Count>0)
        {
            DishType eDish = dishEffectQueue.EatSpeedUpQueue.Peek();
            if (eDish == dish)
            {
                dishEffectQueue.EatSpeedUpQueue.Dequeue();
                CookerCustomerEatSpeedUp();
            }
        }

        if (dishEffectQueue.TipChanceUpQueue.Count > 0)
        {
            DishType tDish = dishEffectQueue.TipChanceUpQueue.Peek();
            if (tDish == dish)
            {
                dishEffectQueue.TipChanceUpQueue.Dequeue();

                CustomerTipChanceUp();
            }
        }
    }

    public void SetCustomer(CustomerStateManager customer)
    {
        this.customer = customer;
    }

    private IEnumerator SleepingChanceCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            if (IsBusy)
            {
                yield return new WaitForSeconds(2.0f);
                continue;
            }
            if (UnityEngine.Random.value < sleepingChance)
            {
                IsBusy = true;
                ChangeState(new ServerSleepingState(this));
                yield return new WaitForSeconds(20.0f);
                continue;
            }

            yield return new WaitForSeconds(2.0f);
        }
    }

   

    public void SetAutoWorkingUI()
    {
        AutoWorkingImg.gameObject.SetActive(true);
    }


    private void ServerDie()
    {
        ChangeState(new ServerGameOverState(this));
    }

    public void CreateDish()
    {
        dishObject = Instantiate(DishDataDB.GetData(dish).DishPrefab, foodSpot);
        tray.gameObject.SetActive(true);
    }

    public void DestroyDish()
    {
        tray.gameObject.SetActive(false);
        Destroy(dishObject);
        dishObject = null;
    }

    public void ToastMessageOn(MessageType messageType)
    {
        toastMessage.ShowMessage(messageType);
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
        animator.applyRootMotion = false;
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
        customerChanged -= CustomerEatSpeedUp;
        customerChanged -= CustomerTipChanceUp;
        GameManager.Instance.Service.Events.Unsubscribe(ServiceEventType.LoopEnded, serviceEnd);
        serviceEnd -= ServerDie;
    }
}
