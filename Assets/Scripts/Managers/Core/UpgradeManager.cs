using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeAvailability
{
    Available,
    InvalidData,
    MaxLevel,
    MarketLevelLocked,
    InsufficientCurrency,
    InsufficientIngredients
}

public partial class UpgradeManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<UpgradeState> upgradeStates = new();
    [SerializeField] private RuntimeStat runtimeStat;
    [SerializeField] private RuntimeLevel runtimeLevel = new();

    private Dictionary<string, UpgradeState> upgradeStateMap = new();
    private Dictionary<HarvestUpgradeType, UpgradeState> harvestStateMap = new();
    private Dictionary<DishType, UpgradeState> dishStateMap = new();
    private Dictionary<EmployeeType, UpgradeState> employeeStateMap = new();
    private Dictionary<FacilityType, UpgradeState> facilityStateMap = new();

    private StatCalculator statCalculator;
    private StockManager stockManager;

    private Action onUpgradeChanged;
    public RuntimeStat RuntimeStat => runtimeStat;
    public RuntimeLevel RuntimeLevel => runtimeLevel;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        statCalculator = new StatCalculator();
        BuildUpgradeStateMaps();
        RefreshRuntimeData();
    }

    private void Start()
    {
        stockManager = GameManager.Instance.StockManager;
    }

    #endregion

    #region Upgrade

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
        RegisterTypedState(state);

        return state;
    }

    public RuntimeStat GetRuntimeStat()
    {
        return runtimeStat;
    }

    public bool TryUpgrade(UpgradeDataSO data)
    {
        UpgradeState state = GetState(data);

        if (state == null
            || GetUpgradeAvailability(data, state.level) != UpgradeAvailability.Available)
            return false;

        if (data is DishUpgradeDataSO dishUpgradeData)
        {
            int targetUpgradeLevel = state.level + 1;
            if (!dishUpgradeData.TryGetRequiredIngredients(
                    targetUpgradeLevel,
                    out List<GroceryAmount> requiredIngredients)
                || !stockManager.TryConsumeGrocery(requiredIngredients))
                return false;
        }
        else if (!state.TryGetCurrentCost(out int requiredCost)
            || !stockManager.TryConsumeCurrency(requiredCost))
        {
            return false;
        }

        state.level++;
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_Upgrade);
        RefreshRuntimeData();
        return true;
    }

    public bool CanUpgrade(UpgradeDataSO data)
    {
        return GetUpgradeAvailability(data) == UpgradeAvailability.Available;
    }

    public UpgradeAvailability GetUpgradeAvailability(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return UpgradeAvailability.InvalidData;

        int currentUpgradeLevel = 0;

        if (upgradeStateMap.TryGetValue(data.Id, out UpgradeState state))
        {
            if (state.data != data)
                return UpgradeAvailability.InvalidData;

            currentUpgradeLevel = state.level;
        }

        return GetUpgradeAvailability(data, currentUpgradeLevel);
    }

    public bool CanUpgradeAtCurrentMarketLevel(UpgradeDataSO data, int currentUpgradeLevel)
    {
        if (data == null || GameManager.Instance == null
            || GameManager.Instance.Market == null)
        {
            return false;
        }

        int targetUpgradeLevel = currentUpgradeLevel + 1;
        if (!data.TryGetRequiredMarketLevel(targetUpgradeLevel, out int requiredLevel))
            return false;

        return GameManager.Instance.Market.MarketData.CurrentLevel >= requiredLevel;
    }

    public bool IsMaxLevel(UpgradeState state)
    {
        return state != null && state.data != null && state.level >= state.data.MaxLevel;
    }

    private UpgradeAvailability GetUpgradeAvailability
        (UpgradeDataSO data, int currentUpgradeLevel)
    {
        if (data == null || stockManager == null)
            return UpgradeAvailability.InvalidData;

        if (currentUpgradeLevel >= data.MaxLevel)
            return UpgradeAvailability.MaxLevel;

        if (!CanUpgradeAtCurrentMarketLevel(data, currentUpgradeLevel))
            return UpgradeAvailability.MarketLevelLocked;

        if (data is DishUpgradeDataSO dishUpgradeData)
        {
            int dishTargetUpgradeLevel = currentUpgradeLevel + 1;
            if (!dishUpgradeData.TryGetRequiredIngredients(
                    dishTargetUpgradeLevel,
                    out List<GroceryAmount> requiredIngredients))
                return UpgradeAvailability.InvalidData;

            if (!stockManager.CanConsumeGrocery(requiredIngredients))
                return UpgradeAvailability.InsufficientIngredients;

            return UpgradeAvailability.Available;
        }

        int targetUpgradeLevel = currentUpgradeLevel + 1;
        if (!data.TryGetRequiredCost(targetUpgradeLevel, out int requiredCost))
            return UpgradeAvailability.InvalidData;

        if (!stockManager.CanConsumeCurrency(requiredCost))
            return UpgradeAvailability.InsufficientCurrency;

        return UpgradeAvailability.Available;
    }

    public bool HasState(UpgradeDataSO data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return false;

        return upgradeStateMap.ContainsKey(data.Id);
    }

    #endregion

    #region Events

    public void SubscribeUpgradeChanged(Action callback)
    {
        onUpgradeChanged += callback;
    }

    public void UnsubscribeUpgradeChanged(Action callback)
    {
        onUpgradeChanged -= callback;
    }

    #endregion

    #region Runtime Calculation

    private void BuildUpgradeStateMaps()
    {
        upgradeStateMap.Clear();
        harvestStateMap.Clear();
        dishStateMap.Clear();
        employeeStateMap.Clear();
        facilityStateMap.Clear();

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
            RegisterTypedState(state);
        }
    }

    private void RegisterTypedState(UpgradeState state)
    {
        switch (state.data)
        {
            case HarvestUpgradeDataSO harvestData
                when (int)harvestData.TargetUpgrade >= 0
                    && (int)harvestData.TargetUpgrade < (int)HarvestUpgradeType.Count:
                harvestStateMap[harvestData.TargetUpgrade] = state;
                break;

            case DishUpgradeDataSO dishData
                when (int)dishData.TargetDish >= 0 && (int)dishData.TargetDish < (int)DishType.Count:
                dishStateMap[dishData.TargetDish] = state;
                break;

            case EmployeeUpgradeDataSO employeeData
                when (int)employeeData.TargetEmployee >= 0
                    && (int)employeeData.TargetEmployee < (int)EmployeeType.Count:
                employeeStateMap[employeeData.TargetEmployee] = state;
                break;

            case FacilityUpgradeDataSO facilityData
                when (int)facilityData.TargetFacility >= 0
                    && (int)facilityData.TargetFacility < (int)FacilityType.Count:
                facilityStateMap[facilityData.TargetFacility] = state;
                break;
        }
    }

    internal void RefreshRuntimeData()
    {
        runtimeStat = statCalculator.Calculate(upgradeStates);
        runtimeLevel = new RuntimeLevel();

        foreach (KeyValuePair<HarvestUpgradeType, UpgradeState> pair in harvestStateMap)
        {
            if (pair.Value != null)
                runtimeLevel.Set(pair.Key, pair.Value.level);
        }

        foreach (KeyValuePair<DishType, UpgradeState> pair in dishStateMap)
        {
            if (pair.Value != null)
                runtimeLevel.Set(pair.Key, pair.Value.level);
        }

        foreach (KeyValuePair<EmployeeType, UpgradeState> pair in employeeStateMap)
        {
            if (pair.Value != null)
                runtimeLevel.Set(pair.Key, pair.Value.level);
        }

        foreach (KeyValuePair<FacilityType, UpgradeState> pair in facilityStateMap)
        {
            if (pair.Value != null)
                runtimeLevel.Set(pair.Key, pair.Value.level);
        }

        onUpgradeChanged?.Invoke();
    }

    #endregion

    #region Save Data

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
            }
        }

        BuildUpgradeStateMaps();
        RefreshRuntimeData();
    }// 저장된 id로 UpgradeData를 다시 찾고 UpgradeState를 복구한다.
    // 복구 후 런타임 스탯과 스킬 레벨을 다시 반영한다.

    public void ResetUpgradeSaveData()
    {
        upgradeStates.Clear();
        BuildUpgradeStateMaps();

        GetState(UpgradeDataDB.GetData(FacilityType.Table_1)).level = 1;
        GetState(UpgradeDataDB.GetData(EmployeeType.Server_1)).level = 1;
        GetState(UpgradeDataDB.GetData(EmployeeType.Cooker_1)).level = 1;
        GetState(UpgradeDataDB.GetData(HarvestUpgradeType.StageLevel)).level = 1;

        RefreshRuntimeData();
    }

    #endregion

}
