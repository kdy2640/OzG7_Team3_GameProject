using System;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public bool IsGameLoopScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.GameLoop; 
     
    private GameLoopEventManager eventManager; 
    private Action<float> OnTick;
    private float timer;
    private bool isRunning = false;
    private bool isPaused = false;

    public float Timer { get { return timer; } }
    public bool IsRunning => isRunning;

    public bool IsPaused => isPaused;
    public GameLoopEventManager Events => eventManager; 
    private void Awake()
    { 
        eventManager = new GameLoopEventManager(); 
    } 
    public void PrepareReveal()
    {
        timer = 0;
        OnTick?.Invoke(Timer);
    }
    public void StartLoop()
    {
        if (!IsGameLoopScene) return; 
        isRunning = true;

        eventManager.Invoke(GameLoopEventType.LoopStarted);
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
        if (!IsGameLoopScene) return;
        isRunning = false;
        eventManager.Invoke(GameLoopEventType.LoopEnded);
    }
    public void Pause()
    {
        isPaused = true;
        eventManager.Invoke(GameLoopEventType.Pause);
    }
    public void UnPause()
    {
        isPaused = false;
        eventManager.Invoke(GameLoopEventType.UnPause);
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
        GameManager.Instance.Scene.RestartScene(SceneType.GameLoop);
    }

}
