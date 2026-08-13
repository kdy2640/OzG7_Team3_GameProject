using UnityEngine;

public class Notification : MonoBehaviour
{
    GameObject noFreeServerMessage;
    private OrderButton orderButton;
    private float timer = 2.0f;

    private void OnEnable()
    {
        noFreeServerMessage = GameObject.FindWithTag("NoFreeServer");
        orderButton = GetComponentInParent<OrderButton>();
        orderButton.NoFreeServer += PlayNotification;
    }

    private void Update()
    {
        if (!noFreeServerMessage.activeSelf)
            return;

        timer -= Time.deltaTime;

        if(timer<=0)
        {
            OffNotification();
        }
    }

    private void PlayNotification()
    {
        timer = 2.0f;
        noFreeServerMessage.SetActive(true);
    }

    private void OffNotification()
    {
        noFreeServerMessage.SetActive(false);
    }

    private void OnDisable()
    {
        orderButton.NoFreeServer -= PlayNotification;
    }
}
