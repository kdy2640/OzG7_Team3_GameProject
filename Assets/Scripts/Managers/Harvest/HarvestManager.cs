using System;
using UnityEngine;

public class HarvestManager : MonoBehaviour
{
    public bool IsGameLoopScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Harvest;
    private float loopDuration = 20f;

    private HarvestEventManager eventManager; 
    private Action<float> OnTick;
    private float timer;
    private bool isRunning = false;

    public float Timer { get { return timer; } }
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
        if (!isRunning)
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
