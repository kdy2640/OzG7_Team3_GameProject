using System;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public bool IsServiceScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Service;
    private float loopDuration = 120f;

    private ServiceEventManager eventManager;
    private SalesResultBuilder resultBuilder;
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
    public ServiceEventManager Events => eventManager;
    public SalesResultBuilder ResultBuilder => resultBuilder;
    public SalesResultData LastSalesResult { get; private set; }

    private void Awake()
    {
        eventManager = new ServiceEventManager();
        resultBuilder = new SalesResultBuilder();
    }

    public void PrepareReveal()
    {
        timer = loopDuration;
        LastSalesResult = null;
        resultBuilder.Reset(GameManager.Instance.Market.MarketData.YesterdaySales);
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
        if (!IsServiceScene) return;
        isRunning = false;

        LastSalesResult = resultBuilder.Build();
        if (LastSalesResult != null)
        {
            GameManager.Instance.Market.MarketData.YesterdaySales =
                LastSalesResult.todaySales;
        }

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
