using System.Collections.Generic;
using UnityEngine;

public static class HarvestDataDB
{
    private const string LoadPath = "SOs/HarvestDataSO";

    private static Dictionary<HarvestType, HarvestDataSO> harvestDataMap;

    public static HarvestDataSO GetData(HarvestType harvestType)
    {
        if (!TryGetData(harvestType, out HarvestDataSO data))
            Debug.LogWarning($"There is no HarvestDataSO. harvestType : {harvestType}");

        return data;
    }

    public static bool TryGetData(HarvestType harvestType, out HarvestDataSO data)
    {
        Initialize();
        return harvestDataMap.TryGetValue(harvestType, out data);
    }

    private static void Initialize()
    {
        if (harvestDataMap != null)
            return;

        harvestDataMap = new Dictionary<HarvestType, HarvestDataSO>();
        HarvestDataSO[] resources = Resources.LoadAll<HarvestDataSO>(LoadPath);

        foreach (HarvestDataSO data in resources)
        {
            if (data == null)
                continue;

            if (data.HarvestType == HarvestType.Count)
            {
                Debug.LogWarning($"{data.name} HarvestDataSO harvestType is Count.");
                continue;
            }

            if (harvestDataMap.ContainsKey(data.HarvestType))
            {
                Debug.LogWarning(
                    $"HarvestDataSO harvestType duplication. harvestType : {data.HarvestType}, SO Name : {data.name}");
                continue;
            }

            harvestDataMap.Add(data.HarvestType, data);
        }
    }
}
