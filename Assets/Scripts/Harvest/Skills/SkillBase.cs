using System;
using UnityEngine;

public class SkillBase
{
    public event Action<float> OnTick;

    public float CoolDownTime { get; }

    private float remainingTime;
    private Action executeAction;

    public SkillBase(float coolDownTime)
    {
        CoolDownTime = Mathf.Max(0f, coolDownTime);
    }

    public void SetExecute(Action action)
    {
        executeAction = action;
    }

    public void Tick()
    {
        if (remainingTime <= 0f)
            return;

        remainingTime = Mathf.Max(
            0f,
            remainingTime - Time.deltaTime);

        OnTick?.Invoke(remainingTime);
    }

    public void Execute()
    {
        if (!CanExecute())
            return;

        remainingTime = CoolDownTime;
        OnTick?.Invoke(remainingTime);
        executeAction.Invoke();
    }

    public bool CanExecute()
    {
        return remainingTime <= 0f
            && executeAction != null;
    }
}
