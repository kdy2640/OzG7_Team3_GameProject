using UnityEngine;

// 직원 모집 또는 강화 실행만 담당합니다.
public sealed class StaffManagementService
{
    private UpgradeManager Upgrade => GameManager.Instance.Upgrade;

    public bool TryRecruitOrUpgrade(EmployeeType type)
    {
        Debug.Log($"[Staff] 레벨업 요청: {type}");

        if (!EmployeeDataDB.TryGetData(type,out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"[Staff] EmployeeData가 없습니다: {type}");

            return false;
        }

        Debug.Log($"[Staff] EmployeeData 찾음: {employeeData.Id}");

        EmployeeUpgradeDataSO upgradeData = UpgradeDataDB.GetData(type);

        if (upgradeData == null)
        {
            Debug.LogWarning($"[Staff] EmployeeUpgradeData가 없습니다: {type}");

            return false;
        }

        Debug.Log($"[Staff] EmployeeUpgradeData 찾음: {upgradeData.Id}");

        bool result = Upgrade.TryUpgrade(upgradeData);

        Debug.Log($"[Staff] TryUpgrade 결과: {result}");

        return result;
    }
}