using UnityEngine;
using UnityEngine.UI;

public class UI_TimerElement : MonoBehaviour
{
    [SerializeField] private Image timerFill;

    private HarvestManager subscribedHarvestManager;
    private ServiceManager subscribedServiceManager;
    private float loopDuration;

    private void OnEnable()
    {
        if (GameManager.Instance == null || GameManager.Instance.Scene == null)
            return;

        switch (GameManager.Instance.Scene.CurrentSceneType)
        {
            case SceneType.Harvest:
                subscribedHarvestManager = GameManager.Instance.Harvest;
                loopDuration = subscribedHarvestManager.LoopDuration;
                subscribedHarvestManager.SubscribeTick(SetTimer);
                SetTimer(subscribedHarvestManager.Timer);
                break;

            case SceneType.Service:
                subscribedServiceManager = GameManager.Instance.Service;
                loopDuration = subscribedServiceManager.LoopDuration;
                subscribedServiceManager.SubscribeTick(SetTimer);
                SetTimer(subscribedServiceManager.Timer);
                break;
        }
    }

    private void OnDisable()
    {
        if (subscribedHarvestManager != null)
        {
            subscribedHarvestManager.UnSubscribeTick(SetTimer);
            subscribedHarvestManager = null;
        }

        if (subscribedServiceManager != null)
        {
            subscribedServiceManager.UnSubscribeTick(SetTimer);
            subscribedServiceManager = null;
        }

        loopDuration = 0f;
    }

    public void SetTimer(float timer)
    {
        if (timerFill == null)
            return;

        timerFill.fillAmount = loopDuration > 0f
            ? Mathf.Clamp01(1f - timer / loopDuration)
            : 0f;
    }
}
