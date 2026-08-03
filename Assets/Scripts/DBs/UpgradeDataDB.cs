using System.Collections.Generic;
using UnityEngine;

public static class UpgradeDataDB
{
    private static Dictionary<string, UpgradeDataSO> upgradeDataMap;

    private static readonly Dictionary<string, string> idMigrationMap = new()
    {
        // id를 바꾼 경우 여기에 연결한다.
        // { "old_id", "new_id" },
    };

    private static readonly string[] LoadPaths =
    {
        "SOs/UpgradeDatas/Harvest",
        "SOs/UpgradeDatas/Dish",
        "SOs/UpgradeDatas/Employee",
        "SOs/UpgradeDatas/Facility"
    };

    public static UpgradeDataSO GetData(string id)
    {
        if (!TryGetData(id, out UpgradeDataSO data))
            Debug.LogWarning($"There is no UpgradeDataSO. id : {id}");

        return data;
    }// id에 맞는 UpgradeDataSO를 반환한다.

    public static bool TryGetData(string id, out UpgradeDataSO data)
    {
        Initialize();

        string migratedId = MigrateId(id);
        return upgradeDataMap.TryGetValue(migratedId, out data);
    }// id로 UpgradeDataSO를 찾는다.
    // id가 바뀐 적 있으면 migration map을 거쳐서 새 id로 찾는다.

    private static string MigrateId(string id)
    {
        if (string.IsNullOrEmpty(id))
            return id;

        if (idMigrationMap.TryGetValue(id, out string migratedId))
            return migratedId;

        return id;
    }// 예전 저장 파일의 old id를 현재 사용하는 new id로 바꿔준다.
    // id가 바뀐 적 없으면 원래 id를 그대로 반환한다.

    private static void Initialize()
    {
        if (upgradeDataMap != null)
            return;

        upgradeDataMap = new Dictionary<string, UpgradeDataSO>();

        foreach (string path in LoadPaths)
        {
            UpgradeDataSO[] resources = Resources.LoadAll<UpgradeDataSO>(path);

            foreach (UpgradeDataSO data in resources)
            {
                if (data == null)
                    continue;

                if (string.IsNullOrEmpty(data.Id))
                {
                    Debug.LogWarning($"{data.name} UpgradeDataSO id is empty.");
                    continue;
                }

                if (upgradeDataMap.ContainsKey(data.Id))
                {
                    Debug.LogWarning(
                        $"UpgradeDataSO id duplication. id : {data.Id}, SO Name : {data.name}");
                    continue;
                }

                upgradeDataMap.Add(data.Id, data);
            }
        }
    }// Resources 폴더 안의 모든 UpgradeDataSO를 찾아 id 기준으로 딕셔너리에 저장한다.
    // 중복 id나 빈 id는 저장하지 않고 경고만 띄운다.
}
