using System;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    private Action<Poolable> returnHandler;

    public abstract void Initialize(PoolArgs args);
    public abstract void ResetState();

    public void RequestReturn()
    {
        returnHandler(this);
    }

    public void SubscribeReturnListener(Action<Poolable> handler)
    {
        returnHandler += handler;
    }

    public void UnsubscribeReturnListener(Action<Poolable> handler)
    {
        returnHandler -= handler;
    }
}
