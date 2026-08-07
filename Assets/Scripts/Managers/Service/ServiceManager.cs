using System;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public bool IsServiceScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Service;
    private float loopDuration = 20f;

    private ServiceEventManager eventManager;
    private Action<float> OnTick;
    private float timer;
    private bool isRunning = false;

    public float Timer { get { return timer; } }
    public bool IsRunning => isRunning;
    public ServiceEventManager Events => eventManager;

    private void Awake()
    {
        eventManager = new ServiceEventManager();
    }

    public void PrepareReveal()
    {
        timer = loopDuration;
        OnTick?.Invoke(Timer);
    }

    public void StartLoop()
    {
        if (!IsServiceScene) return;
        isRunning = true;

        eventManager.Invoke(ServiceEventType.LoopStarted);
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
        if (!IsServiceScene) return;
        isRunning = false;
        eventManager.Invoke(ServiceEventType.LoopEnded);
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
        GameManager.Instance.Scene.RestartScene(SceneType.Service);
    }
}
