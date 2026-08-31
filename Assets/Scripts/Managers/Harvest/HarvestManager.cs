using System;
using UnityEngine;

public class HarvestManager : MonoBehaviour
{
    public bool IsGameLoopScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Harvest;
    private float loopDuration = 20f;

    private HarvestEventManager eventManager; 
    private Action<float> OnTick;
    [SerializeField, Min(0f)] private float timeWarningThreshold = 5f;
    [SerializeField] private float timer;
    [SerializeField] private bool isPause;
    private bool isRunning = false;
    private bool timeWarningPlayed;

    public float Timer { get { return timer; } }
    public float LoopDuration => loopDuration;
    public bool IsPause
    {
        get => isPause;
        set => isPause = value;
    }
    public bool IsRunning => isRunning;
    public HarvestEventManager Events => eventManager; 


    private void Awake()
    {
        eventManager = new HarvestEventManager(); 
    }
    public void PrepareReveal()
    {
        HarvestRuntimeStat harvestStat =
            GameManager.Instance?.Upgrade?.RuntimeStat?.Harvest;

        if (harvestStat != null)
        {
            loopDuration = harvestStat.Get(HarvestStatType.TruckFuel);
        }

        timer = loopDuration;
        timeWarningPlayed = false;
        OnTick?.Invoke(Timer);
    }
    public void StartLoop()
    {
        if (!IsGameLoopScene) return;
        isRunning = true;

        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Harvest_SessionStart);
        eventManager.Invoke(HarvestEventType.LoopStarted);
    }

    private void Update()
    {
        if (!isRunning || isPause)
            return;

        timer -= Time.deltaTime;
        OnTick?.Invoke(Timer);

        if (!timeWarningPlayed
            && timer > 0f
            && timer <= timeWarningThreshold)
        {
            timeWarningPlayed = true;
            GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Harvest_TimeWarning);
        }

        if (timer <= 0f)
            EndLoop();
    }

    public void EndLoop()
    {
        if (!isRunning) return;
        if (!IsGameLoopScene) return;
        isRunning = false;

        GameManager.Instance.Utility.Audio.StopLoopSFX(SFXType.Harvest_Grind);
        GameManager.Instance.Utility.Audio.StopLoopSFX(SFXType.Harvest_TractorEngine);
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Harvest_SessionEnd);
        eventManager.Invoke(HarvestEventType.LoopEnded);
    }

    public void SubscribeTick(Action<float> ev)
    {
        OnTick += ev;
    }
    public void UnSubscribeTick(Action<float> ev)
    {
        OnTick -= ev;
    }
    public void Restart()
    {
        GameManager.Instance.Scene.RestartScene(SceneType.Harvest);
    }

}
