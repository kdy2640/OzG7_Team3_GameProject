using UnityEngine;
using UnityEngine.UI;

public class SleepingButton : MonoBehaviour
{
    private ServerStateManager stateManager;
    [SerializeField] private Image fillIMG;

    private float sleepingTime = 10.0f;
    private float timer;
    private bool isSelected;

    private void OnEnable()
    {
        timer = sleepingTime;
        isSelected = false;
        stateManager = GetComponentInParent<ServerStateManager>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if( timer < 0 )
        {
            timer = 0;
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_NegativeEventResolve);
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
        if (!isSelected)
        {
            isSelected = true;
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_NegativeEventSelect);
        }

        timer -= 2.0f;
    }
}
