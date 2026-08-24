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

    [SerializeField]private bool leftSeatClosed;

    public bool LeftSeatClosed  => leftSeatClosed;

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

        leftSeatClosed = true;
    }

    

    public bool HasEmptySeat()
    {
        if(leftSeatClosed)
        {
            return rightCustomer == null ? true : false;
        }
        return (leftCustomer == null) || (rightCustomer == null);
    }


    public void OpenLeftSeat()
    {
        leftSeatClosed = false;
    }

    public Transform ReserveSeat(CustomerStateManager customer)
    {
        if (rightCustomer == null)
        {
            rightCustomer = customer;
            return rightSeatPoint;
        }

        if (leftSeatClosed)
        {
            return null;
        }

        if (leftCustomer == null)
        {
            leftCustomer = customer;
            return leftSeatPoint;
        }

        Debug.Log("자리 예약 오류");
        return null;
    }

    public void ReleaseSeat(CustomerStateManager customer)
    {
        if(customer.CurrentTable == null)
        {
            return;
        }
        if (rightCustomer == customer)
        {
            rightCustomer = null;
        }
        if (leftCustomer == customer)
        {
            leftCustomer = null;
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
