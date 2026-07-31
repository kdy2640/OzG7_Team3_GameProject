using System;
using UnityEngine;

public class HarvestManager : MonoBehaviour
{
    public bool IsHarvestScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Harvest; 
     
    private HarvestEventManager eventManager; 
    private Action<float> OnTick;
    private float timer;
    private bool isRunning = false;
    private bool isPaused = false;

    public float Timer { get { return timer; } }
    public bool IsRunning => isRunning;

    public bool IsPaused => isPaused;
    public HarvestEventManager Events => eventManager; 
    private void Awake()
    { 
        eventManager = new HarvestEventManager(); 
    } 
    public void PrepareReveal()
    {
        timer = 0;
        OnTick?.Invoke(Timer);
    }
    public void StartLoop()
    {
        if (!IsHarvestScene) return; 
        isRunning = true;

        eventManager.Invoke(HarvestEventType.LoopStarted);
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (isPaused) return;

        timer += Time.deltaTime; 
        OnTick?.Invoke(Timer);

        if (timer <= 0f)
            EndLoop();
    }

    public void EndLoop()
    {
        if (!isRunning) return;
        if (!IsHarvestScene) return;
        isRunning = false;
        eventManager.Invoke(HarvestEventType.LoopEnded);
    }
    public void Pause()
    {
        isPaused = true;
        eventManager.Invoke(HarvestEventType.Pause);
    }
    public void UnPause()
    {
        isPaused = false;
        eventManager.Invoke(HarvestEventType.UnPause);
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
