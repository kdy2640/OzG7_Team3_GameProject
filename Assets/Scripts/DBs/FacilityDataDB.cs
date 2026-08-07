using System.Collections.Generic;
using UnityEngine;

public static class FacilityDataDB
{
    private const string LoadPath = "SOs/DefinitionDatas/FacilityDataSO";

    private static Dictionary<FacilityType, FacilityDataSO> facilityDataMap;

    public static int Count
    {
        get
        {
            Initialize();
            return facilityDataMap.Count;
        }
    }

    public static FacilityDataSO GetData(FacilityType facilityType)
    {
        if (!TryGetData(facilityType, out FacilityDataSO data))
            Debug.LogWarning($"There is no FacilityDataSO. facilityType : {facilityType}");

        return data;
    }

    public static bool TryGetData(FacilityType facilityType, out FacilityDataSO data)
    {
        Initialize();
        return facilityDataMap.TryGetValue(facilityType, out data);
    }

    private static void Initialize()
    {
        if (facilityDataMap != null)
            return;

        facilityDataMap = new Dictionary<FacilityType, FacilityDataSO>();
        FacilityDataSO[] resources = Resources.LoadAll<FacilityDataSO>(LoadPath);

        foreach (FacilityDataSO data in resources)
        {
            if (data == null)
                continue;

            if (data.FacilityType == FacilityType.Count)
            {
                Debug.LogWarning($"{data.name} FacilityDataSO facilityType is Count.");
                continue;
            }

            if (facilityDataMap.ContainsKey(data.FacilityType))
            {
                Debug.LogWarning(
                    $"FacilityDataSO facilityType duplication. facilityType : {data.FacilityType}, SO Name : {data.name}");
                continue;
            }

            facilityDataMap.Add(data.FacilityType, data);
        }
    }
}
