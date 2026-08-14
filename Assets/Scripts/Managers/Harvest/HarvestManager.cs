using System;
using UnityEngine;

public class HarvestManager : MonoBehaviour
{
    public bool IsGameLoopScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Harvest;
    private float loopDuration = 20f;

    private HarvestEventManager eventManager; 
    private Action<float> OnTick;
    [SerializeField] private float timer;
    [SerializeField] private bool isPause;
    private bool isRunning = false;

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
        timer = loopDuration;  // + GameManager.Instance.Upgrade.GetRuntimeStat().ExtraDuration;
        OnTick?.Invoke(Timer);
    }
    public void StartLoop()
    {
        if (!IsGameLoopScene) return;
        isRunning = true;

        eventManager.Invoke(HarvestEventType.LoopStarted);
    }

    private void Update()
    {
        if (!isRunning || isPause)
            return;

        timer -= Time.deltaTime;
        OnTick?.Invoke(Timer);

        if (timer <= 0f)
            EndLoop();
    }

    public void EndLoop()
    {
        if (!isRunning) return;
        if (!IsGameLoopScene) return;
        isRunning = false;
        eventManager.Invoke(HarvestEventType.LoopEnded);
        GameManager.Instance.Scene.ChangeScene(SceneType.Hub);
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
