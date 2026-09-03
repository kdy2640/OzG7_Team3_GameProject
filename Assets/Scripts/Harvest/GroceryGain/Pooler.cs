using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolArgs
{
}

public class Pooler<T> : MonoBehaviour where T : Poolable
{
    [SerializeField] protected T prefab;

    private readonly Queue<T> pool = new();
    private Transform poolContainer;

    public virtual T Get(PoolArgs args)
    {
        T item = pool.Count > 0
            ? pool.Dequeue()
            : Create();

        item.gameObject.SetActive(true);
        item.Initialize(args);
        return item;
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T item = Create();
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
        }
    }

    private T Create()
    {
        EnsurePoolContainer();

        T item = Instantiate(prefab, poolContainer);
        item.SubscribeReturnListener(Return);
        return item;
    }

    private void Return(Poolable poolable)
    {
        T item = (T)poolable;
        item.ResetState();
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    private void EnsurePoolContainer()
    {
        if (poolContainer != null)
        {
            return;
        }

        GameObject container = new($"[Pool] {typeof(T).Name}");
        poolContainer = container.transform;
    }
}
