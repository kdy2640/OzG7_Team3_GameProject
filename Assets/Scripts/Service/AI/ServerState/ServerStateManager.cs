using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ServerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform servePoint;
    [SerializeField] private Transform kitchen;
    [SerializeField] private Transform waitPoint;

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
                    orderButton.OnClick();
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
            DishType EDish = dishEffectQueue.EatSpeedUpQueue.Peek();
            if (EDish == dish)
            {
                dishEffectQueue.EatSpeedUpQueue.Dequeue();
                CookerCustomerEatSpeedUp();
            }
        }

        if (dishEffectQueue.TipChanceUpQueue.Count > 0)
        {
            DishType TDish = dishEffectQueue.TipChanceUpQueue.Peek();
            if (TDish == dish)
            {
                dishEffectQueue.TipChanceUpQueue.Dequeue();

                CustomerTipChanceUp();
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