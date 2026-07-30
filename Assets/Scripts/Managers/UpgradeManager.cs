using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeState> upgradeStates = new();
    [SerializeField] private RuntimeStat runtimeStat;

    private Dictionary<string, UpgradeState> upgradeStateMap = new();

    private StatCalculator statCalculator;
    private CurrencyManager currencyManager;

    public RuntimeStat RuntimeStat => runtimeStat;

    private void Awake()
    {
        statCalculator = new StatCalculator();
        BuildUpgradeStateMap();
        RecalculateRuntimeStat();
    }

    private void Start()
    {
        currencyManager = GameManager.Instance.CurrencyManager;
    }
     
    public UpgradeState GetState(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.id))
            return null;

        if (upgradeStateMap.TryGetValue(data.id, out UpgradeState state))
        {
            if (state.data != data)
            {
                Debug.LogError($"중복된 UpgradeData id가 있습니다: {data.id}");
                return null;
            }

            return state;
        }

        state = new UpgradeState
        {
            data = data,
            level = 0
        };

        upgradeStates.Add(state);
        upgradeStateMap.Add(data.id, state);

        return state;
    }

    public RuntimeStat GetRuntimeStat()
    {
        return runtimeStat;
    }

    public bool TryUpgrade(UpgradeDataSO data)
    {
        UpgradeState state = GetState(data);

        if (state == null || IsMaxLevel(state))
            return false;

        if (!currencyManager.TrySpend(state.GetCurrentCost()))
            return false;

        state.level++;
        RecalculateRuntimeStat();
        return true;
    }

    public bool IsMaxLevel(UpgradeState state)
    {
        return state != null && state.data != null && state.level >= state.data.maxLevel;
    }

    public bool HasState(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.id))
            return false;

        return upgradeStateMap.ContainsKey(data.id);
    }

    private void BuildUpgradeStateMap()
    {
        upgradeStateMap.Clear();

        for (int i = 0; i < upgradeStates.Count; i++)
        {
            UpgradeState state = upgradeStates[i];

            if (state?.data == null || string.IsNullOrEmpty(state.data.id))
            {
                upgradeStates.RemoveAt(i);
                i--;
                continue;
            }

            if (upgradeStateMap.ContainsKey(state.data.id))
            {
                Debug.LogError($"중복된 UpgradeData id가 있습니다: {state.data.id}");
                upgradeStates.RemoveAt(i);
                i--;
                continue;
            }

            upgradeStateMap.Add(state.data.id, state);
        }
    }

    private void RecalculateRuntimeStat()
    {
        runtimeStat = statCalculator.Calculate(upgradeStates);
    }
}
