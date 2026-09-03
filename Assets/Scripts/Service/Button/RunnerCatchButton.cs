using Unity.VisualScripting;
using UnityEngine;

public class RunnerCatchButton : MonoBehaviour
{

    private CustomerStateManager customer;
    private ServerStateManager server;
    private ServerList serverList;

    private bool isClicked = false;

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
        if (isClicked) return;
        if(serverList.TryAllocCatch(customer, out server))
        {
            isClicked = true;
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_NegativeEventSelect);
            customer.AiMove.StopMove();
            customer.AnimSetIdle();
            customer.AiMove.SetDirection(server.transform.position);
        }
        else
        {
            // 서버 바쁨 메시지
        }
    }
}
