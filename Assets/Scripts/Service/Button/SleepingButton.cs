using UnityEngine;
using UnityEngine.UI;

public class SleepingButton : MonoBehaviour
{
    private ServerStateManager stateManager;
    [SerializeField] private Image fillIMG;

    private float sleepingTime = 10.0f;
    private float timer;

    private void OnEnable()
    {
        timer = sleepingTime;
        stateManager = GetComponentInParent<ServerStateManager>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if( timer < 0 )
        {
            timer = 0;
            stateManager.ChangeState(new ServerGetBackState(stateManager));
            
            return;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        fillIMG.fillAmount = timer / sleepingTime;
    }

    public void OnClick()
    {
        timer -= 2.0f;
    }
}
