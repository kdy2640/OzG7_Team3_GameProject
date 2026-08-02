using System;
using System.Collections.Generic;

public enum HarvestEventType
{
    BeforeLoopStarted,
    LoopStarted,
    Pause,
    UnPause,
    LoopEnded
}

public interface IHarvestEventSubscribable
{
    void Subscribe(HarvestEventType eventType, Action action);
    void Unsubscribe(HarvestEventType eventType, Action action);
}

public class HarvestEventManager : IHarvestEventSubscribable
{
    private readonly Dictionary<HarvestEventType, Action> events = new();

    public void Subscribe(HarvestEventType eventType, Action action)
    {
        if (!events.ContainsKey(eventType))
            events[eventType] = null;

        events[eventType] += action;
    }

    public void Unsubscribe(HarvestEventType eventType, Action action)
    {
        if (!events.ContainsKey(eventType))
            return;

        events[eventType] -= action;
    }

    public void Invoke(HarvestEventType eventType)
    {
        if (!events.TryGetValue(eventType, out Action action))
            return;

        action?.Invoke();
    }
}
