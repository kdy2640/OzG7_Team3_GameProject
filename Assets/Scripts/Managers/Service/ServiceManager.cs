using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public bool IsServiceScene => GameManager.Instance.Scene.CurrentSceneType == SceneType.Service;

    private ServiceEventManager eventManager;
    private SalesResultBuilder resultBuilder;
    [SerializeField] private ServiceProgress progress = new();
    [SerializeField] private bool isPause;
    private bool isRunning = false;

    public bool IsPause
    {
        get => isPause;
        set => isPause = value;
    }
    public bool IsRunning => isRunning;
    public ServiceEventManager Events => eventManager;
    public ServiceProgress Progress => progress;
    public SalesResultBuilder ResultBuilder => resultBuilder;
    public SalesResultData LastSalesResult { get; private set; }

    private void Awake()
    {
        eventManager = new ServiceEventManager();
        resultBuilder = new SalesResultBuilder();
    }

    public void PrepareReveal()
    {
        progress.Reset();
        isRunning = false;
        LastSalesResult = null;
        resultBuilder.Reset(GameManager.Instance.Market.MarketData.YesterdaySales);
        eventManager.Invoke(ServiceEventType.BeforeLoopStarted);
    }

    public void StartLoop()
    {
        if (!IsServiceScene) return;
        isRunning = true;

        eventManager.Invoke(ServiceEventType.LoopStarted);
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

    public void Restart()
    {
        GameManager.Instance.Scene.RestartScene(SceneType.Service);
    }
}
