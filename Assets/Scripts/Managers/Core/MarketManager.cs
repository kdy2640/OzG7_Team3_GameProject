using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    [SerializeField] private MarketData marketData = new();
    [SerializeField] private List<EmployeeBase> employees = new();
    [SerializeField] private List<FacilityBase> facilities = new();

    public IReadOnlyList<EmployeeType> UnlockedEmployees => marketData.UnlockedEmployees;
    public IReadOnlyList<FacilityType> UnlockedFacilities => marketData.UnlockedFacilities;
    public List<DishType> SelectedDishes => marketData.SelectedDishes;
    public int DishSelectionLimit => marketData.DishSelectionLimit;

    public IReadOnlyList<EmployeeBase> Employees => employees;
    public IReadOnlyList<FacilityBase> Facilities => facilities;

    public int CurrentBusinessDay
    {
        get => marketData.CurrentBusinessDay;
        set => marketData.CurrentBusinessDay = value;
    }

    private void Start()
    {
        GameManager.Instance.Upgrade.SubscribeRuntimeStatRefresh(OnRuntimeStatRefresh);
        Refresh();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
            return;

        GameManager.Instance.Upgrade.UnSubscribeRuntimeStatRefresh(OnRuntimeStatRefresh);
    }

    public void Refresh()
    {
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
            dishSelectionLimit = marketData.DishSelectionLimit
        };

        saveData.unlockedEmployees.AddRange(marketData.UnlockedEmployees);
        saveData.unlockedFacilities.AddRange(marketData.UnlockedFacilities);
        saveData.selectedDishes.AddRange(marketData.SelectedDishes);

        return saveData;
    }

    public void LoadMarketSaveData(MarketSaveData saveData)
    {
        marketData = saveData == null
            ? new MarketData()
            : new MarketData(
                Mathf.Max(0, saveData.currentBusinessDay),
                Mathf.Max(0, saveData.dishSelectionLimit),
                saveData.unlockedEmployees,
                saveData.unlockedFacilities,
                saveData.selectedDishes);

        Refresh();
    }

    public void ResetMarketSaveData()
    {
        marketData = new MarketData();
        Refresh();
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
