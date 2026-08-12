using System.Collections.Generic;
using UnityEngine;

public static class LevelMissionGroupDB
{
    private const string LoadPath = "SOs/LevelMissionGroups";

    private static Dictionary<int, LevelMissionGroupSO> missionGroupMap;

    public static LevelMissionGroupSO GetData(int level)
    {
        if (!TryGetData(level, out LevelMissionGroupSO missionGroup))
            Debug.LogWarning($"There is no LevelMissionGroupSO. level : {level}");

        return missionGroup;
    }

    public static bool TryGetData(int level, out LevelMissionGroupSO missionGroup)
    {
        Initialize();
        return missionGroupMap.TryGetValue(level, out missionGroup);
    }

    private static void Initialize()
    {
        if (missionGroupMap != null)
            return;

        missionGroupMap = new Dictionary<int, LevelMissionGroupSO>();
        LevelMissionGroupSO[] missionGroups =
            Resources.LoadAll<LevelMissionGroupSO>(LoadPath);

        foreach (LevelMissionGroupSO missionGroup in missionGroups)
        {
            if (missionGroup == null)
                continue;

            if (missionGroupMap.ContainsKey(missionGroup.Level))
            {
                Debug.LogWarning(
                    $"LevelMissionGroupSO level duplication. level : {missionGroup.Level}, SO Name : {missionGroup.name}");
                continue;
            }

            missionGroupMap.Add(missionGroup.Level, missionGroup);
        }
    }
}
