using System.Collections.Generic;
using UnityEngine;

public static class LevelDataDB
{
    private const string LoadPath = "CSVs/LevelDataSheet";

    private static Dictionary<int, LevelData> levelDataMap;

    public static LevelData GetData(int level)
    {
        if (!TryGetData(level, out LevelData data))
            Debug.LogWarning($"There is no LevelData. level : {level}");

        return data;
    }

    public static bool TryGetData(int level, out LevelData data)
    {
        Initialize();
        return levelDataMap.TryGetValue(level, out data);
    }

    private static void Initialize()
    {
        if (levelDataMap != null)
            return;

        levelDataMap = new Dictionary<int, LevelData>();
        List<Dictionary<string, object>> rows = CSVReader.Read(LoadPath);

        foreach (Dictionary<string, object> row in rows)
        {
            int level = GetInt(row, "Level");

            if (levelDataMap.ContainsKey(level))
            {
                Debug.LogWarning($"LevelData level duplication. level : {level}");
                continue;
            }

            LevelData data = new()
            {
                Level = level,
                MaxDishLimit = GetInt(row, "MaxDishLimit"),
                MaxEXPLimit = GetInt(row, "MaxEXPLimit")
            };

            levelDataMap.Add(level, data);
        }
    }

    private static int GetInt(Dictionary<string, object> row, string key)
    {
        if (!row.TryGetValue(key, out object value))
        {
            Debug.LogWarning($"LevelDataSheet column does not exist. column : {key}");
            return 0;
        }

        if (value is int intValue)
            return intValue;

        if (int.TryParse(value?.ToString(), out int parsedValue))
            return parsedValue;

        Debug.LogWarning($"LevelDataSheet value is not a valid int. column : {key}, value : {value}");
        return 0;
    }
}
