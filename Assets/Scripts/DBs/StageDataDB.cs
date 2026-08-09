using System.Collections.Generic;
using UnityEngine;

public static class StageDataDB
{
    private const string LoadPath = "SOs/DefinitionDatas/StageDataSO";

    private static Dictionary<StageType, StageDataSO> stageDataMap;

    public static StageDataSO GetData(StageType stageType)
    {
        if (!TryGetData(stageType, out StageDataSO data))
            Debug.LogWarning($"There is no StageDataSO. stageType : {stageType}");

        return data;
    }

    public static bool TryGetData(StageType stageType, out StageDataSO data)
    {
        Initialize();
        return stageDataMap.TryGetValue(stageType, out data);
    }

    private static void Initialize()
    {
        if (stageDataMap != null)
            return;

        stageDataMap = new Dictionary<StageType, StageDataSO>();
        StageDataSO[] resources = Resources.LoadAll<StageDataSO>(LoadPath);

        foreach (StageDataSO data in resources)
        {
            if (data == null)
                continue;

            if (data.StageType == StageType.Count)
            {
                Debug.LogWarning($"{data.name} StageDataSO stageType is Count.");
                continue;
            }

            if (stageDataMap.ContainsKey(data.StageType))
            {
                Debug.LogWarning(
                    $"StageDataSO stageType duplication. stageType : {data.StageType}, SO Name : {data.name}");
                continue;
            }

            stageDataMap.Add(data.StageType, data);
        }
    }
}
