using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HPHandler))]
[RequireComponent(typeof(HarvestPresenter))]
[RequireComponent(typeof(AnimalStateController))]
public sealed class HarvestActor : MonoBehaviour
{
    [SerializeField] private HPHandler hpHandler;
    [SerializeField] private HarvestPresenter presenter;
    [SerializeField] private HarvestMover mover;
    [SerializeField] private AnimalStateController animalStateController;

    private HarvestDataSO harvestDataSO;
    private HarvestEmployeeResolver employeeResolver;
    private ChunkRegistry registry;
    private bool isInitialized;
    private bool isDying;


    public void Init(
        HarvestType type,
        StageType stageType,
        Transform player,
        GridChunkHandler gridChunkHandler,
        HarvestEmployeeResolver resolver)
    {
        registry = gridChunkHandler.Registry;
        employeeResolver = resolver;
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
                stageType,
                gridChunkHandler,
                harvestDataSO.HarvestType == HarvestType.Pig);

            Animator animator = solid.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animalStateController.enabled = true;
            animalStateController.Init(
                player,
                mover,
                animator,
                harvestDataSO.AnimalStat,
                harvestDataSO.AnimatorController);
        }
        else
        {
            if (mover != null)
            {
                mover.enabled = false;
            }

            animalStateController.enabled = false;
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

        if (harvestDataSO.IsMove)
        {
            animalStateController.PlayHit();
        }
        else
        {
            presenter.PlayHit();
        }

        hpHandler.TakeDamage(damage);
    }

    private void OnDied()
    {
        isDying = true;
        registry.Unregister(transform);
        GameManager.Instance.StockManager.AddGrocery(harvestDataSO.Rewards);
        employeeResolver.ResolveHarvested(harvestDataSO);
        if (harvestDataSO.IsMove)
        {
            animalStateController.SetState(AnimalStateType.Dead);
        }
        else
        {
            presenter.PlayDeath();
        }
    }

#if UNITY_EDITOR
    private void Reset()
    {
        hpHandler = GetComponent<HPHandler>();
        presenter = GetComponent<HarvestPresenter>();
        mover = GetComponent<HarvestMover>();
        animalStateController = GetComponent<AnimalStateController>();
    }
#endif
}
