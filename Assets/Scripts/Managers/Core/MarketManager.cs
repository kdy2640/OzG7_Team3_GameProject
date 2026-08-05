using System;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    [SerializeField] private MarketData marketData = new();
    [SerializeField] private LevelData levelData = new();
    [SerializeField] private List<EmployeeBase> employees = new();
    [SerializeField] private List<FacilityBase> facilities = new();

    private Action onMarketDataChanged;

    public MarketData Data => marketData;
    public LevelData LevelData => levelData;
    public IReadOnlyList<EmployeeBase> Employees => employees;
    public IReadOnlyList<FacilityBase> Facilities => facilities;

    private void Awake()
    {
        marketData ??= new MarketData();
        SubscribeMarketData();
    }

    private void Start()
    {
        GameManager.Instance.Upgrade.SubscribeRuntimeStatRefresh(OnRuntimeStatRefresh);
        Refresh();
    }

    private void OnDestroy()
    {
        if (marketData != null)
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;

        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
            return;

        GameManager.Instance.Upgrade.UnSubscribeRuntimeStatRefresh(OnRuntimeStatRefresh);
    }

    public void Refresh()
    {
        levelData = LevelDataDB.GetData(marketData.CurrentLevel) ?? new LevelData();

        employees.Clear();
        facilities.Clear();

        int employeeCount = EmployeeDataDB.Count;
        EmployeeRuntimeStat employeeRuntimeStat = GameManager.Instance.Upgrade.RuntimeStat.Employee;

        for (int index = 0; index < employeeCount; index++)
        {
            EmployeeType employeeType = (EmployeeType)index;
            EmployeeDataSO dataSO = EmployeeDataDB.GetData(employeeType);

            if (dataSO != null)
            {
                EmployeeBase employee = new EmployeeBase(dataSO)
                {
                    NowLevel = employeeRuntimeStat.GetLevel(employeeType)
                };

                employees.Add(employee);
            }
        }

        int facilityCount = FacilityDataDB.Count;
        FacilityRuntimeStat facilityRuntimeStat = GameManager.Instance.Upgrade.RuntimeStat.Facility;

        for (int index = 0; index < facilityCount; index++)
        {
            FacilityType facilityType = (FacilityType)index;
            FacilityDataSO dataSO = FacilityDataDB.GetData(facilityType);

            if (dataSO != null)
            {
                FacilityBase facility = new FacilityBase(dataSO)
                {
                    NowLevel = facilityRuntimeStat.GetLevel(facilityType)
                };

                facilities.Add(facility);
            }
        }
    }

    public MarketSaveData CreateMarketSaveData()
    {
        MarketSaveData saveData = new()
        {
            currentBusinessDay = marketData.CurrentBusinessDay,
            currentLevel = marketData.CurrentLevel,
            currentEXP = marketData.CurrentEXP
        };

        saveData.selectedDishes.AddRange(marketData.SelectedDishes);

        return saveData;
    }

    public void LoadMarketSaveData(MarketSaveData saveData)
    {
        MarketData loadedData = saveData == null
            ? new MarketData()
            : new MarketData(
                Mathf.Max(0, saveData.currentBusinessDay),
                Mathf.Max(0, saveData.currentLevel),
                Mathf.Max(0, saveData.currentEXP),
                saveData.selectedDishes);

        ReplaceMarketData(loadedData);
        Refresh();
        NotifyMarketDataChanged();
    }

    public void ResetMarketSaveData()
    {
        ReplaceMarketData(new MarketData());
        Refresh();
        NotifyMarketDataChanged();
    }

    public void SubscribeMarketDataChanged(Action callback)
    {
        onMarketDataChanged += callback;
    }

    public void UnsubscribeMarketDataChanged(Action callback)
    {
        onMarketDataChanged -= callback;
    }

    private void ReplaceMarketData(MarketData newMarketData)
    {
        if (marketData != null)
            marketData.OnMarketDataChanged -= HandleMarketDataChanged;

        marketData = newMarketData ?? new MarketData();
        SubscribeMarketData();
    }

    private void SubscribeMarketData()
    {
        marketData.OnMarketDataChanged -= HandleMarketDataChanged;
        marketData.OnMarketDataChanged += HandleMarketDataChanged;
    }

    private void HandleMarketDataChanged()
    {
        NotifyMarketDataChanged();
    }

    private void NotifyMarketDataChanged()
    {
        onMarketDataChanged?.Invoke();
    }

    private void OnRuntimeStatRefresh(RuntimeStat runtimeStat)
    {
        for (int index = 0; index < employees.Count; index++)
        {
            EmployeeBase employee = employees[index];

            if (employee != null)
                employee.NowLevel = runtimeStat.Employee.GetLevel((EmployeeType)index);
        }

        for (int index = 0; index < facilities.Count; index++)
        {
            FacilityBase facility = facilities[index];

            if (facility != null)
                facility.NowLevel = runtimeStat.Facility.GetLevel((FacilityType)index);
        }
    }
}
