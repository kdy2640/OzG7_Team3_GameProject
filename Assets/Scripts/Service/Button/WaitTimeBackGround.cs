using UnityEngine;
using UnityEngine.UI;

public class WaitTimeBackGround : MonoBehaviour
{
    [SerializeField] private Image fillImg;

    private float maxValue;
    private float currentValue;
    
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
    }
}
