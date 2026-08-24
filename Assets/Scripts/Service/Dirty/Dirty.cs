using UnityEngine;

public class Dirty : MonoBehaviour
{
    private CustomerStateManager customer;
    public CustomerStateManager Customer => customer;
    
    public void SetCustomer(CustomerStateManager customer)
    {
        this.customer = customer;
    }
}
