using System.Collections.Generic;
using UnityEngine;

public static class DishDataDB
{
    private static Dictionary<DishType, DishDataSO> dishDataMap;

    private static readonly string[] LoadPaths =
    {
        "SOs/DishDataSO",
    };

    // 요리 종류로 데이터를 가져올 때 사용.
    public static DishDataSO GetData(DishType dish)
    {
        if (!TryGetData(dish, out DishDataSO data))
            Debug.LogWarning($"There is no DishDataSO. dish : {dish}");

        return data;
    }

    // 요리 데이터가 존재하는지 확인하면서 가져올 때 사용.
    public static bool TryGetData(DishType dish, out DishDataSO data)
    {
        Initialize();
        return dishDataMap.TryGetValue(dish, out data);
    }

    private static void Initialize()
    {
        if (dishDataMap != null)
            return;

        dishDataMap = new Dictionary<DishType, DishDataSO>();

        foreach (string path in LoadPaths)
        {
            DishDataSO[] resources = Resources.LoadAll<DishDataSO>(path);

            foreach (DishDataSO data in resources)
            {
                if (data == null)
                    continue;

                if (data.dish == DishType.Count)
                {
                    Debug.LogWarning($"{data.name} DishDataSO dish is Count.");
                    continue;
                }

                if (dishDataMap.ContainsKey(data.dish))
                {
                    Debug.LogWarning(
                        $"DishDataSO dish duplication. dish : {data.dish}, SO Name : {data.name}");
                    continue;
                }

                dishDataMap.Add(data.dish, data);
            }
        }
    }
}
