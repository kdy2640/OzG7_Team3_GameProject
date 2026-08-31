using System;
using UnityEngine;

[Serializable]
public sealed class ServiceProgress
{
    public float Value { get; private set; }
    public event Action<float> ValueChanged;

    public void Reset()
    {
        SetValue(0f);
    }

    public void SetValue(float value)
    {
        float nextValue = Mathf.Clamp01(value);

        if (Mathf.Approximately(Value, nextValue))
            return;

        Value = nextValue;
        ValueChanged?.Invoke(Value);
    }
}
