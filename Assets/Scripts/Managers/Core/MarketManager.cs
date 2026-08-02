using System.Collections.Generic;
using UnityEngine;

// TODO: Own the persisted market state (day, employees, and facilities).
public class MarketManager : MonoBehaviour
{
    [SerializeField] private int currentBusinessDay;
    [SerializeField] private List<EmployeeBase> employees = new();
    [SerializeField] private List<FacilityBase> facilities = new();

    public int CurrentBusinessDay
    {
        get => currentBusinessDay;
        set => currentBusinessDay = value;
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
        return new MarketSaveData(currentBusinessDay);
    }

    public void LoadMarketSaveData(MarketSaveData saveData)
    {
        currentBusinessDay = saveData == null
            ? 0
            : Mathf.Max(0, saveData.currentBusinessDay);

        Refresh();
    }

    public void ResetMarketSaveData()
    {
        currentBusinessDay = 0;
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
