using UnityEngine;

// 직원 모집 또는 강화 실행만 담당합니다.
public sealed class StaffManagementService
{
    private UpgradeManager Upgrade => GameManager.Instance.Upgrade;

    public bool TryRecruitOrUpgrade(EmployeeType type)
    {
        if (!EmployeeDataDB.TryGetData(type, out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData가 없습니다: {type}");
            return false;
        }

        EmployeeUpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(employeeData.EmployeeType);

        if (upgradeData == null)
        {
            Debug.LogWarning($"EmployeeUpgradeData가 없습니다: {type}");
            return false;
        }

        return Upgrade.TryUpgrade(upgradeData);
    }
}