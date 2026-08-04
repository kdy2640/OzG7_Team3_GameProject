using System;
using System.Collections.Generic;

public enum ServiceEventType
{
    BeforeLoopStarted,
    LoopStarted,
    Pause,
    UnPause, 
    LoopEnded
}

public interface IServiceEventSubscribable
{
    void Subscribe(ServiceEventType eventType, Action action);
    void Unsubscribe(ServiceEventType eventType, Action action);
}

public class ServiceEventManager : IServiceEventSubscribable
{
    private readonly Dictionary<ServiceEventType, Action> events = new();

    public void Subscribe(ServiceEventType eventType, Action action)
    {
        if (!events.ContainsKey(eventType))
            events[eventType] = null;

        events[eventType] += action;
    }

    public void Unsubscribe(ServiceEventType eventType, Action action)
    {
        if (!events.ContainsKey(eventType))
            return;

        events[eventType] -= action;
    }

    public void Invoke(ServiceEventType eventType)
    {
        if (!events.TryGetValue(eventType, out Action action))
            return;

        action?.Invoke();
    }
}
