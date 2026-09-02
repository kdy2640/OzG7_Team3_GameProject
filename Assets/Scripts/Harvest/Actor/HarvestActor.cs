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
        hpHandler.Init(harvestDataSO.HP);

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
                harvestDataSO.AnimatorController,
                harvestDataSO.HarvestType);
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
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Harvest_Collect);

        if (harvestDataSO.HarvestType == HarvestType.Pig)
        {
            GrantGoldenPigReward();
            GameManager.Instance.Utility.Audio.PlaySFX(
                SFXType.Harvest_GoldenPigCollected);
        }
        else
        {
            GameManager.Instance.StockManager.AddGrocery(harvestDataSO.Rewards);
            employeeResolver.ResolveHarvested(harvestDataSO);
        }

        if (harvestDataSO.IsMove)
        {
            animalStateController.SetState(AnimalStateType.Dead);
        }
        else
        {
            presenter.PlayDeath();
        }
    }

    private void GrantGoldenPigReward()
    {
        int radarLevel = GameManager.Instance.Upgrade.RuntimeLevel.Get(
            HarvestUpgradeType.GoldenPigRadar);

        int firstAmount;
        int secondAmount;
        int thirdAmount;
        int goldAmount;

        switch (radarLevel)
        {
            case 1:
                firstAmount = 10;
                secondAmount = 5;
                thirdAmount = 3;
                goldAmount = 20;
                break;
            case 2:
                firstAmount = 20;
                secondAmount = 10;
                thirdAmount = 5;
                goldAmount = 50;
                break;
            case 3:
                firstAmount = 30;
                secondAmount = 15;
                thirdAmount = 10;
                goldAmount = 100;
                break;
            case 4:
                firstAmount = 50;
                secondAmount = 30;
                thirdAmount = 20;
                goldAmount = 500;
                break;
            default:
                Debug.LogError(
                    $"[HarvestActor] Invalid golden pig radar level: {radarLevel}",
                    this);
                return;
        }

        StageDataSO stageData = StageDataDB.GetData(
            (StageType)(radarLevel - 1));
        GameManager.Instance.StockManager.AddGrocery(
            new GroceryAmount(stageData.RewardList[0], firstAmount));
        GameManager.Instance.StockManager.AddGrocery(
            new GroceryAmount(stageData.RewardList[1], secondAmount));
        GameManager.Instance.StockManager.AddGrocery(
            new GroceryAmount(stageData.RewardList[2], thirdAmount));
        GameManager.Instance.StockManager.AddCurrency(goldAmount);
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
