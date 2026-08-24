using Unity.VisualScripting;
using UnityEngine;

public class RunnerCatchButton : MonoBehaviour
{

    private CustomerStateManager customer;
    private ServerStateManager server;
    private ServerList serverList;

    private void OnEnable()
    {
        serverList = FindAnyObjectByType<ServerList>();
    }

    public void Initialize(CustomerStateManager customer)
    {
        this.customer = customer;
    }
    public void OnClick()
    {
        if(serverList.TryAllocCatch(customer, out server))
        {
            customer.AiMove.StopMove();
            customer.AiMove.SetDirection(server.transform.position);
            customer.ChangeState(new CustomerCaughtState(customer));

        }
        else
        {
            // 서버 바쁨 메시지
        }
    }
}
