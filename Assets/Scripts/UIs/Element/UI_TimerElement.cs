using UnityEngine;
using UnityEngine.UI;

public class UI_TimerElement : MonoBehaviour
{
    [SerializeField] private Image timerFill;

    [Header("Service Progress")]
    [SerializeField, Min(0.1f)] private float serviceProgressFollowDuration = 10f;

    private HarvestManager subscribedHarvestManager;
    private ServiceManager subscribedServiceManager;
    private float loopDuration;
    private float targetServiceProgress;
    private float displayedServiceProgress;

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
                subscribedServiceManager.Progress.ValueChanged += SetProgress;
                SetProgress(subscribedServiceManager.Progress.Value);
                displayedServiceProgress = targetServiceProgress;

                if (timerFill != null)
                    timerFill.fillAmount = displayedServiceProgress;
                break;
        }
    }

    private void Update()
    {
        if (subscribedServiceManager == null
            || subscribedServiceManager.IsPause
            || timerFill == null)
        {
            return;
        }

        float step = Time.deltaTime / serviceProgressFollowDuration;
        displayedServiceProgress = Mathf.MoveTowards(
            displayedServiceProgress,
            targetServiceProgress,
            step);
        timerFill.fillAmount = displayedServiceProgress;
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
            subscribedServiceManager.Progress.ValueChanged -= SetProgress;
            subscribedServiceManager = null;
        }

        loopDuration = 0f;
        targetServiceProgress = 0f;
        displayedServiceProgress = 0f;
    }

    public void SetTimer(float timer)
    {
        if (timerFill == null)
            return;

        timerFill.fillAmount = loopDuration > 0f
            ? Mathf.Clamp01(1f - timer / loopDuration)
            : 0f;
    }

    private void SetProgress(float progress)
    {
        float nextProgress = Mathf.Clamp01(progress);

        if (nextProgress < targetServiceProgress)
            displayedServiceProgress = nextProgress;

        targetServiceProgress = nextProgress;
    }
}
