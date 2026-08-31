using System.Collections.Generic;
using UnityEngine;

public static class ItemDataDB
{
    private const string LoadPath = "SOs/DefinitionDatas/ItemDataSO";

    private static Dictionary<ItemType, ItemDataSO> itemDataMap;

    public static ItemDataSO GetData(ItemType itemType)
    {
        if (!TryGetData(itemType, out ItemDataSO data))
        {
            Debug.LogWarning(
                $"There is no ItemDataSO. itemType : {itemType}");
        }

        return data;
    }

    public static bool TryGetData(ItemType itemType, out ItemDataSO data)
    {
        Initialize();
        return itemDataMap.TryGetValue(itemType, out data);
    }

    private static void Initialize()
    {
        if (itemDataMap != null)
        {
            return;
        }

        itemDataMap = new Dictionary<ItemType, ItemDataSO>();
        ItemDataSO[] resources = Resources.LoadAll<ItemDataSO>(LoadPath);

        foreach (ItemDataSO data in resources)
        {
            if (data == null)
            {
                continue;
            }

            if (data.ItemType == ItemType.Count)
            {
                Debug.LogWarning($"{data.name} ItemDataSO itemType is Count.");
                continue;
            }

            if (itemDataMap.ContainsKey(data.ItemType))
            {
                Debug.LogWarning(
                    $"ItemDataSO itemType duplication. itemType : {data.ItemType}, SO Name : {data.name}");
                continue;
            }

            itemDataMap.Add(data.ItemType, data);
        }
    }
}
