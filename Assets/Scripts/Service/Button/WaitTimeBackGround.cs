using UnityEngine;
using UnityEngine.UI;

public class WaitTimeBackGround : MonoBehaviour
{
    [SerializeField] private Image fillImg;
    [SerializeField] private CustomerStateManager stateManager;
    private float maxValue;
    private float currentValue;

    private void OnEnable()
    {
        if(stateManager == null)
            stateManager = GetComponentInParent<CustomerStateManager>();
    }

    public void SetWaitTimeUI(float timer)
    {
        maxValue = timer;
    }

    public void RunWaitTimeUI(float timer)
    {
        currentValue = timer;
        UpdateWaitTimeUI();
    }

    private void UpdateWaitTimeUI()
    {
        fillImg.fillAmount = currentValue / maxValue;

        if(currentValue <= maxValue/3)
        {
            DangerNotation();
            stateManager.IsLateReceive = true;
        }
    }

    private void DangerNotation()
    {
        fillImg.color = Color.red;
    }
}
