using System.Collections.Generic;
using UnityEngine;

public static class GroceryDataDB
{
    private const string LoadPath = "SOs/DefinitionDatas/GroceryDataSO";

    private static Dictionary<GroceryType, GroceryDataSO> groceryDataMap;

    public static GroceryDataSO GetData(GroceryType grocery)
    {
        if (!TryGetData(grocery, out GroceryDataSO data))
            Debug.LogWarning($"There is no GroceryDataSO. grocery : {grocery}");

        return data;
    }

    public static bool TryGetData(GroceryType grocery, out GroceryDataSO data)
    {
        Initialize();
        return groceryDataMap.TryGetValue(grocery, out data);
    }

    private static void Initialize()
    {
        if (groceryDataMap != null)
            return;

        groceryDataMap = new Dictionary<GroceryType, GroceryDataSO>();
        GroceryDataSO[] resources = Resources.LoadAll<GroceryDataSO>(LoadPath);

        foreach (GroceryDataSO data in resources)
        {
            if (data == null)
                continue;

            if (data.Grocery == GroceryType.Count)
            {
                Debug.LogWarning($"{data.name} GroceryDataSO grocery is Count.");
                continue;
            }

            if (groceryDataMap.ContainsKey(data.Grocery))
            {
                Debug.LogWarning(
                    $"GroceryDataSO grocery duplication. grocery : {data.Grocery}, SO Name : {data.name}");
                continue;
            }

            groceryDataMap.Add(data.Grocery, data);
        }
    }
}
