using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HPHandler))]
[RequireComponent(typeof(HarvestPresenter))]
public sealed class HarvestActor : MonoBehaviour
{
    [SerializeField] private HPHandler hpHandler;
    [SerializeField] private HarvestPresenter presenter;
    [SerializeField] private HarvestMover mover;

    private HarvestDataSO harvestDataSO;
    private ChunkRegistry registry;
    private bool isInitialized;
    private bool isDying;


    public void Init(
        HarvestType type,
        StageType stageType,
        Transform player,
        GridChunkHandler gridChunkHandler)
    {
        registry = gridChunkHandler.Registry;
        harvestDataSO = HarvestDataDB.GetData(type);
        isDying = false;

        if (hpHandler == null)
        {
            hpHandler = GetComponent<HPHandler>();
        }

        if (presenter == null)
        {
            presenter = GetComponent<HarvestPresenter>();
        }

        hpHandler.SubscribeDying(OnDied); 
        hpHandler.Init();

        GameObject solid = Instantiate(harvestDataSO.SolidPrefab, transform);
        presenter.Init(solid);
        isInitialized = true;

        if (harvestDataSO.IsMove)
        {
            if (mover == null)
                mover = GetComponent<HarvestMover>();

            if (mover == null)
            {
                Debug.LogError("[HarvestActor] HarvestMover is not assigned.", this);
                return;
            }

            mover.Init(
                player,
                harvestDataSO.Speed,
                stageType,
                gridChunkHandler);
        }
        else if (mover != null)
        {
            mover.enabled = false;
        }

        if (gameObject.activeInHierarchy)
        {
            registry.Register(transform);
        }
    }

    private void OnEnable()
    {
        if (isInitialized && registry != null)
        {
            registry.Register(transform);
        }
    }

    private void OnDestroy()
    {
        if (hpHandler != null && presenter != null)
        { 
            hpHandler.UnSubscribeDying(OnDied);
        }
    }

    private void OnDisable()
    {
        if (registry != null)
        {
            registry.Unregister(transform);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || !gameObject.activeInHierarchy || isDying)
        {
            return;
        }

        presenter.PlayHit();
        hpHandler.TakeDamage(damage);
    }

    private void OnDied()
    {
        isDying = true;
        registry.Unregister(transform);
        GameManager.Instance.StockManager.AddGrocery(harvestDataSO.Rewards);
        presenter.PlayDeath();
    }

#if UNITY_EDITOR
    private void Reset()
    {
        hpHandler = GetComponent<HPHandler>();
        presenter = GetComponent<HarvestPresenter>();
        mover = GetComponent<HarvestMover>();
    }
#endif
}
