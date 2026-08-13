using UnityEngine;

public class Table : MonoBehaviour
{
    [Header("Seat")]
    [SerializeField] private Transform leftSeatPoint;
    [SerializeField] private Transform rightSeatPoint;

    [Header("Serve Point")]
    [SerializeField] private Transform leftServePoint;
    [SerializeField] private Transform rightServePoint;

    [SerializeField] private TableManager tableManager;

    private CustomerStateManager leftCustomer;
    private CustomerStateManager rightCustomer;

    private void Awake()
    {
        if (tableManager == null)
        {
            tableManager = FindAnyObjectByType<TableManager>();
        }

        if (leftSeatPoint == null)
        {
            leftSeatPoint = transform.Find("SeatPoint Left");
        }
        if (rightSeatPoint == null)
        {
            rightSeatPoint = transform.Find("SeatPoint Right");
        }
        if (leftServePoint == null)
        {
            leftServePoint = transform.Find("ServePoint Left");
        }
        if ( rightServePoint == null)
        {
            rightServePoint = transform.Find("ServePoint Right");
        }

        leftSeatPoint.Rotate(0f, 90f, 0f);
        rightSeatPoint.Rotate(0f, -90f, 0f);
    }

    public bool HasEmptySeat()
    {
        return (leftCustomer == null) || (rightCustomer == null);
    }

    public Transform ReserveSeat(CustomerStateManager customer)
    {
        if (leftCustomer == null)
        {
            leftCustomer = customer;
            return leftSeatPoint;
        }

        if (rightCustomer == null)
        {
            rightCustomer = customer;
            return rightSeatPoint;
        }

        Debug.Log("자리 예약 오류");
        return null;
    }

    public void ReleaseSeat(CustomerStateManager customer)
    {
        if (leftCustomer == customer)
        {
            leftCustomer = null;
        }
        else if (rightCustomer == customer)
        {
            rightCustomer = null;
        }

        if(tableManager.IsThereAnyWaiting())
        {
            tableManager.GetSeatForWaitingCustomer();
        }
    }

    public Transform GetServePoint(Transform seat)
    {
        if (seat == leftSeatPoint)
            return leftServePoint;

        if (seat == rightSeatPoint)
            return rightServePoint;

        return null;
    }

    public bool HasCustomer(CustomerStateManager customer)
    {
        return leftCustomer == customer || rightCustomer == customer;
    }
}
