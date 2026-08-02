using System.Collections.Generic;
using UnityEngine;

public static class EmployeeDataDB
{
    private const string LoadPath = "SOs/EmployeeDataSO";

    private static Dictionary<EmployeeType, EmployeeDataSO> employeeDataMap;

    public static int Count
    {
        get
        {
            Initialize();
            return employeeDataMap.Count;
        }
    }

    public static EmployeeDataSO GetData(EmployeeType employeeType)
    {
        if (!TryGetData(employeeType, out EmployeeDataSO data))
            Debug.LogWarning($"There is no EmployeeDataSO. employeeType : {employeeType}");

        return data;
    }

    public static bool TryGetData(EmployeeType employeeType, out EmployeeDataSO data)
    {
        Initialize();
        return employeeDataMap.TryGetValue(employeeType, out data);
    }

    private static void Initialize()
    {
        if (employeeDataMap != null)
            return;

        employeeDataMap = new Dictionary<EmployeeType, EmployeeDataSO>();
        EmployeeDataSO[] resources = Resources.LoadAll<EmployeeDataSO>(LoadPath);

        foreach (EmployeeDataSO data in resources)
        {
            if (data == null)
                continue;

            if (data.employeeType == EmployeeType.Count)
            {
                Debug.LogWarning($"{data.name} EmployeeDataSO employeeType is Count.");
                continue;
            }

            if (employeeDataMap.ContainsKey(data.employeeType))
            {
                Debug.LogWarning(
                    $"EmployeeDataSO employeeType duplication. employeeType : {data.employeeType}, SO Name : {data.name}");
                continue;
            }

            employeeDataMap.Add(data.employeeType, data);
        }
    }
}
