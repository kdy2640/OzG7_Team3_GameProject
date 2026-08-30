using UnityEngine;

public class Table : MonoBehaviour
{
    [Header("Table Position")]
    [SerializeField] private Transform tableTransform;

    [Header("Seat")]
    [SerializeField] private Transform leftSeatPoint;
    [SerializeField] private Transform rightSeatPoint;
    [SerializeField] private Transform leftSeatPos = null;
    [SerializeField] private Transform rightSeatPos = null;


    [Header("Serve Point")]
    [SerializeField] private Transform leftServePoint;
    [SerializeField] private Transform rightServePoint;
    [SerializeField] private Transform leftServePos = null;
    [SerializeField] private Transform rightServePos = null;

    [Header("Food Point")]
    [SerializeField] private Transform leftFoodPoint;
    [SerializeField] private Transform rightFoodPoint;
    [SerializeField] private Transform leftFoodPos = null;
    [SerializeField] private Transform rightFoodPos = null;

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
        if (leftFoodPoint == null)
        {
            leftFoodPoint = transform.Find("FoodPoint Left");
        }
        if (rightFoodPoint == null)
        {
            rightFoodPoint = transform.Find("FoodPoint Right");
        }

        leftSeatClosed = true;
    }



    private void OnValidate()
    {
        if(tableTransform!= null)
        transform.position = tableTransform.position;

        //if(leftSeatPos.position != null && rightSeatPos.position != null)
        //{
        //    leftSeatPoint.position = leftSeatPos.position;
        //    rightSeatPoint.position = rightSeatPos.position;
        //}
        
        //if(leftServePos.position != null && rightServePos.position != null)
        //{
        //    leftServePoint.position = leftServePos.position;
        //    rightServePoint.position = rightServePos.position;
        //}

        //if (leftFoodPos.position != null && rightFoodPos.position != null)
        //{
        //    leftFoodPoint.position = leftFoodPos.position;
        //    rightFoodPoint.position = rightFoodPos.position;
        //}
        
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
            tableManager.TryGetSeatForWaitingCustomer();
            return;
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

    public Transform GetFoodSpot(CustomerStateManager customer)
    {
        if(leftCustomer == customer)
        {
            return leftFoodPoint;
        }
        if(rightCustomer == customer)
        {
            return rightFoodPoint;
        }
        return null;
    }

    public bool HasCustomer(CustomerStateManager customer)
    {
        return leftCustomer == customer || rightCustomer == customer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.brown;
        Gizmos.DrawCube(transform.position, new Vector3(2f, 1f, 2f));
    }
}
