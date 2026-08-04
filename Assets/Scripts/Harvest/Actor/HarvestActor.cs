using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HPHandler))]
[RequireComponent(typeof(HarvestPresenter))]
public sealed class HarvestActor : MonoBehaviour
{
    [SerializeField] private HPHandler hpHandler;
    [SerializeField] private HarvestPresenter presenter;

    private HarvestDataSO harvestDataSO;


    public void Init(HarvestType type)
    {
        harvestDataSO = HarvestDataDB.GetData(type);

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
    }

    private void OnDestroy()
    {
        if (hpHandler != null && presenter != null)
        { 
            hpHandler.UnSubscribeDying(OnDied);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || !gameObject.activeInHierarchy)
        {
            return;
        }

        presenter.PlayHit();
        hpHandler.TakeDamage(damage);
    }

    private void OnDied()
    {
        GameManager.Instance.StockManager.AddGrocery(harvestDataSO.Rewards);
        presenter.Disappear();
    }

#if UNITY_EDITOR
    private void Reset()
    {
        hpHandler = GetComponent<HPHandler>();
        presenter = GetComponent<HarvestPresenter>();
    }
#endif
}
