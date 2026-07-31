using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HPHandler))]
[RequireComponent(typeof(CropPresenter))]
public sealed class CropActor : MonoBehaviour
{
    [SerializeField] private HPHandler hpHandler;
    [SerializeField] private CropPresenter presenter;

    private void Awake()
    {
        if (hpHandler == null)
        {
            hpHandler = GetComponent<HPHandler>();
        }

        if (presenter == null)
        {
            presenter = GetComponent<CropPresenter>();
        }

        hpHandler.SubscribeDying(presenter.Disappear);
        hpHandler.Init();
    }

    private void OnDestroy()
    {
        if (hpHandler != null && presenter != null)
        {
            hpHandler.UnSubscribeDying(presenter.Disappear);
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

#if UNITY_EDITOR
    private void Reset()
    {
        hpHandler = GetComponent<HPHandler>();
        presenter = GetComponent<CropPresenter>();
    }
#endif
}
