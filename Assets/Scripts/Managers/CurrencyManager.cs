using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CurrencyManager : MonoBehaviour
{
    [FormerlySerializedAs("NowCurrency")]
    [SerializeField, Min(0)] private int nowCurrency;

    private Action onCurrencyChanged;

    public int NowCurrency => nowCurrency;

    public int GetCurrency()
    {
        return nowCurrency;
    }

    public bool HasCost(int cost)
    {
        return cost >= 0 && nowCurrency >= cost;
    }

    public bool TrySpend(int cost)
    {
        if (!HasCost(cost))
            return false;

        SetCurrency(nowCurrency - cost);
        return true;
    }

    public void Add(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("CurrencyManager.Add에는 0 이상의 값만 전달할 수 있습니다.");
            return;
        }

        long addedCurrency = (long)nowCurrency + amount;
        SetCurrency((int)Math.Min(addedCurrency, int.MaxValue));
    }

    public void SetCurrency(int amount)
    {
        SetCurrency(amount, false);
    }

    public void SubscribeCurrencyChange(Action callback)
    {
        onCurrencyChanged += callback;
    }

    public void UnsubscribeCurrencyChange(Action callback)
    {
        onCurrencyChanged -= callback;
    }

    private void SetCurrency(int amount, bool forceNotify)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (!forceNotify && nowCurrency == clampedAmount)
            return;

        nowCurrency = clampedAmount;
        onCurrencyChanged?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Debug/Clear Currency")]
    private void ClearCurrency()
    {
        SetCurrency(0, true);
    }
#endif
}
