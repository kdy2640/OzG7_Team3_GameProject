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
    [SerializeField] private SleepingButton sleepingButton;
    [SerializeField] private Image AutoWorkingImg;

    [SerializeField] private float baseSpeed = 2;
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

    

    public AIMove AiMove => aiMove;
    public Transform ServePoint => servePoint;
    public Transform Kitchen => kitchen;
    public Transform WaitPoint => waitPoint;
    public Animator Animator => animator;
    public CustomerStateManager Customer => customer;
    public DishType Dish => dish;
    public int Level => level; 
    public float ServeTime => serveTime;
    public float ReceiveFoodTime => receiveFoodTime;
    public SleepingButton SleepingButton => sleepingButton;
    public bool isAutoWorking = false;

    [SerializeField] private IState currentState;



    private void Awake()
    {
        kitchen = FindFirstObjectByType<Kitchen>().transform;
        aiMove = gameObject.GetComponent<AIMove>();
        animator = gameObject.GetComponentInChildren<Animator>();
        animator.applyRootMotion = false;
        dishEffectQueue = FindFirstObjectByType<DishEffectQueue>();
        speed = baseSpeed;
        UpdateStatus();
    }

    private void Start()
    {
        ChangeState(new ServerGetBackState(this));
        StartCoroutine(SleepingChanceCo());
        StartCoroutine(AutoWorkingUICo());
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
                    orderButton.IsAutoServing = true;
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

    private IEnumerator AutoWorkingUICo()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if(isAutoWorking)
            {
                AutoWorkingImg.gameObject.SetActive(true);
            }
            else
            {
                AutoWorkingImg.gameObject.SetActive(false);
            }
        }
    }


    private void OnDisable()
    {
        Destroy(this.gameObject);
        customerChanged -= CustomerEatSpeedUp;
        customerChanged -= CustomerTipChanceUp;
    }
}
