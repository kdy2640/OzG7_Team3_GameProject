using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeState> upgradeStates = new();
    [SerializeField] private List<UpgradeState> temporaryUpgradeStates = new();
    [SerializeField] private RuntimeStat runtimeStat;

    private Dictionary<string, UpgradeState> upgradeStateMap = new();
    private Dictionary<string, UpgradeState> temporaryUpgradeStateMap = new();

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

        Dictionary<string, UpgradeState> targetMap =
            data.IsTemporary ? temporaryUpgradeStateMap : upgradeStateMap;
        Dictionary<string, UpgradeState> otherMap =
            data.IsTemporary ? upgradeStateMap : temporaryUpgradeStateMap;

        if (otherMap.ContainsKey(data.id))
        {
            Debug.LogError(
                $"UpgradeData id '{data.id}'가 영구/임시 업그레이드 양쪽에 사용되고 있습니다.");
            return null;
        }

        if (targetMap.TryGetValue(data.id, out UpgradeState state))
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

        if (data.IsTemporary)
            temporaryUpgradeStates.Add(state);
        else
            upgradeStates.Add(state);

        targetMap.Add(data.id, state);

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

        if (!data.IsTemporary && !currencyManager.TrySpend(state.GetCurrentCost()))
            return false;

        state.level++;
        RecalculateRuntimeStat();
        return true;
    }

    public void ClearTemporaryUpgrades()
    {
        temporaryUpgradeStates.Clear();
        temporaryUpgradeStateMap.Clear();
        RecalculateRuntimeStat();
    }

    public bool IsMaxLevel(UpgradeState state)
    {
        return state != null && state.data != null && state.level >= state.data.maxLevel;
    }

    public bool HasState(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.id))
            return false;

        Dictionary<string, UpgradeState> targetMap =
            data.IsTemporary ? temporaryUpgradeStateMap : upgradeStateMap;

        return targetMap.ContainsKey(data.id);
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

            if (state.data.IsTemporary)
            {
                Debug.LogError(
                    $"Temporary UpgradeData가 영구 상태 리스트에 들어 있습니다: {state.data.id}");
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
        runtimeStat = statCalculator.Calculate(upgradeStates, temporaryUpgradeStates);
    }
}
