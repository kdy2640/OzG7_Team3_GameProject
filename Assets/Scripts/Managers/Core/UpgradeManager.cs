using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeState> upgradeStates = new();
    [SerializeField] private RuntimeStat runtimeStat;

    private Dictionary<string, UpgradeState> upgradeStateMap = new();

    private StatCalculator statCalculator;
    private StockManager stockManager;

    private Action<RuntimeStat> OnRuntimeStatRefresh;
    public RuntimeStat RuntimeStat => runtimeStat;

    private void Awake()
    {
        statCalculator = new StatCalculator();
        BuildUpgradeStateMap();
        RecalculateRuntimeStat();
    }

    private void Start()
    {
        stockManager = GameManager.Instance.StockManager;
    }

    public UpgradeState GetState(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;

        if (upgradeStateMap.TryGetValue(data.Id, out UpgradeState state))
        {
            if (state.data != data)
            {
                Debug.LogError($"중복된 UpgradeData id가 있습니다: {data.Id}");
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
        upgradeStateMap.Add(data.Id, state);

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

        if (!stockManager.TryConsumeCurrency(state.GetCurrentCost()))
            return false;

        state.level++;
        RecalculateRuntimeStat();
        return true;
    }

    public bool IsMaxLevel(UpgradeState state)
    {
        return state != null && state.data != null && state.level >= state.data.MaxLevel;
    }

    public bool HasState(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return false;

        return upgradeStateMap.ContainsKey(data.Id);
    }

    public void SubscribeRuntimeStatRefresh(Action<RuntimeStat> ev)
    {
        OnRuntimeStatRefresh += ev;
    }
    public void UnSubscribeRuntimeStatRefresh(Action<RuntimeStat> ev)
    {
        OnRuntimeStatRefresh -= ev;
    }



    private void BuildUpgradeStateMap()
    {
        upgradeStateMap.Clear();

        for (int i = 0; i < upgradeStates.Count; i++)
        {
            UpgradeState state = upgradeStates[i];

            if (state?.data == null || string.IsNullOrEmpty(state.data.Id))
            {
                upgradeStates.RemoveAt(i);
                i--;
                continue;
            }

            if (upgradeStateMap.ContainsKey(state.data.Id))
            {
                Debug.LogError($"중복된 UpgradeData id가 있습니다: {state.data.Id}");
                upgradeStates.RemoveAt(i);
                i--;
                continue;
            }

            upgradeStateMap.Add(state.data.Id, state);
        }
    }

    private void RecalculateRuntimeStat()
    {
        runtimeStat = statCalculator.Calculate(upgradeStates);
        OnRuntimeStatRefresh?.Invoke(runtimeStat);
    }



    public List<UpgradeSaveData> CreateUpgradeSaveData()
    {
        List<UpgradeSaveData> saveData = new();

        foreach (UpgradeState state in upgradeStates)
        {
            if (state == null)
                continue;

            if (state.data == null)
                continue;

            if (string.IsNullOrEmpty(state.data.Id))
                continue;

            saveData.Add(new UpgradeSaveData(state.data.Id, state.level));
        }

        return saveData;
    }// 현재 upgradeStates에서 저장할 데이터만 뽑아낸다.
    // SO 자체는 저장하지 않고 UpgradeData의 id와 level만 저장한다.

    public void LoadUpgradeSaveData(List<UpgradeSaveData> saveData)
    {
        upgradeStates.Clear();
        upgradeStateMap.Clear();

        if (saveData != null)
        {
            foreach (UpgradeSaveData savedState in saveData)
            {
                if (savedState == null)
                    continue;

                if (string.IsNullOrEmpty(savedState.id))
                    continue;

                UpgradeDataSO data = UpgradeDataDB.GetData(savedState.id);

                if (data == null)
                    continue;

                UpgradeState state = new()
                {
                    data = data,
                    level = Mathf.Clamp(savedState.level, 0, data.MaxLevel)
                };

                upgradeStates.Add(state);
                upgradeStateMap.Add(data.Id, state);
            }
        }

        RecalculateRuntimeStat();
    }// 저장된 id로 UpgradeData를 다시 찾고 UpgradeState를 복구한다.
    // 복구 후 런타임 스탯과 스킬 레벨을 다시 반영한다.

    public void ResetUpgradeSaveData()
    {

        upgradeStates.Clear();
        upgradeStateMap.Clear();

        RecalculateRuntimeStat();
    }

}
