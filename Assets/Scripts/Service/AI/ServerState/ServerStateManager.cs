using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ServerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform servePoint;
    [SerializeField] private Transform kitchen;
    [SerializeField] private Transform waitPoint;

    private DishType dish;

    private Renderer renderer;
    public Renderer Renderer => renderer;

    private CustomerStateManager customer;

    public bool IsBusy = false;

    public AIMove AiMove => aiMove;
    public Transform ServePoint => servePoint;
    public Transform Kitchen => kitchen;
    public Transform WaitPoint => waitPoint;
    public DishType Dish => dish;

    [SerializeField] private IState currentState;

    private void Awake()
    {
        renderer = gameObject.GetComponent<Renderer>();
        kitchen = FindFirstObjectByType<Kitchen>().transform;
        aiMove = gameObject.GetComponent<AIMove>();
    }

    private void OnEnable()
    {
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

        this.dish = dish;

        servePoint = customer.CurrentTable.GetServePoint(customer.Seat);

        ChangeState(new ServerMoveToTableState(this, aiMove, servePoint));
    }

    public void GiveFood()
    {
        customer.foodReceived?.Invoke();
    }
}