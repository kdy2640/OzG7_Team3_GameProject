using System;
using UnityEngine;

public class SkillBase
{
    public event Action<float> OnTick;

    public float CoolDownTime { get; private set; }
    public bool IsUnlocked => isUnlocked;

    private float remainingTime;
    private int remainingUseCount;
    private bool isUnlocked;
    private Action executeAction;

    public SkillBase(
        float coolDownTime,
        bool unlocked = true,
        int useCount = -1)
    {
        Configure(coolDownTime, unlocked, useCount);
    }

    public void Configure(
        float coolDownTime,
        bool unlocked,
        int useCount = -1)
    {
        CoolDownTime = Mathf.Max(0f, coolDownTime);
        isUnlocked = unlocked;
        remainingUseCount = useCount < 0
            ? -1
            : Mathf.Max(0, useCount);
        remainingTime = 0f;
        OnTick?.Invoke(remainingTime);
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

        if (remainingUseCount > 0)
        {
            remainingUseCount--;
        }

        remainingTime = CoolDownTime;
        OnTick?.Invoke(remainingTime);
        executeAction.Invoke();
    }

    public bool CanExecute()
    {
        return isUnlocked
            && remainingUseCount != 0
            && remainingTime <= 0f
            && executeAction != null;
    }
}
