using UnityEngine;

public class ServerStateManager : MonoBehaviour
{
    [SerializeField] private AIMove aiMove;
    [SerializeField] private Transform servePoint;
    [SerializeField] private Transform kitchen;
    [SerializeField] private Transform waitPoint;

    private DishType dish;

    private Renderer renderer;
    public Renderer Renderer => renderer;



    public bool IsBusy = false;
    public AIMove AiMove => aiMove;

    public Transform ServePoint => servePoint;

    public Transform Kitchen => kitchen;

    public Transform WaitPoint => waitPoint;

    public DishType Dish => dish;

    private IState currentState;

    private void OnEnable()
    {
        renderer = gameObject.GetComponent<Renderer>();
        kitchen = GameObject.FindWithTag("Kitchen").transform;
        aiMove.StopMove();
        ChangeState(new ServerIdleState(this));
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
        this.dish = dish;

        servePoint = customer.CurrentTable.GetServePoint(customer.Seat);
        ChangeState(new ServerMoveToKitchenState(this, aiMove, kitchen));
    }
}