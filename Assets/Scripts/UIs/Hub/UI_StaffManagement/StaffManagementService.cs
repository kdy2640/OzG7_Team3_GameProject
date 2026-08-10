using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 직원 관리 화면에서 필요한 게임 데이터 조회 및
/// 모집/강화 실행을 담당합니다.
/// UI 자체에는 관여하지 않습니다.
/// </summary>
public sealed class StaffManagementService
{
    private UpgradeManager Upgrade => GameManager.Instance.Upgrade;
    private StockManager StockManager => GameManager.Instance.StockManager;

    /// <summary>
    /// 직원 목록 UI에 표시할 데이터를 생성합니다.
    /// </summary>
    public List<StaffCardUIData> CreateCardDataList()
    {
        var result = new List<StaffCardUIData>();

        foreach (EmployeeType type in Enum.GetValues(typeof(EmployeeType)))
        {
            if (type == EmployeeType.Count) continue;

            if (!EmployeeDataDB.TryGetData(type, out EmployeeDataSO employeeData))
                continue;

            int level = Upgrade.GetLevel(type);

            EmployeeUpgradeDataSO upgradeData = GetUpgradeData(type);

            bool canPay = false;

            if (upgradeData != null && level < employeeData.MaxLevel)
            {
                int cost = upgradeData.GetCosts(level);

                canPay = StockManager.CanConsumeCurrency(cost);
            }

            StaffCardState state;

            if (level == 0)
            {
                state = canPay ? StaffCardState.CanRecruit : StaffCardState.Locked;
            }
            else if (level >= employeeData.MaxLevel)
            {
                state = StaffCardState.Normal;
            }
            else
            {
                state = canPay ? StaffCardState.CanUpgrade : StaffCardState.Normal;
            }

            result.Add(new StaffCardUIData
            {
                type = type,
                level = level,
                state = state
            });
        }

        return result;
    }

    /// <summary>
    /// 선택된 직원의 상세 정보 UI 데이터를 생성합니다.
    /// </summary>
    public StaffInfoUIData CreateInfoData(EmployeeType type)
    {
        if (!EmployeeDataDB.TryGetData(type, out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData가 없습니다: {type}");
            return null;
        }

        int level = Upgrade.GetLevel(type);

        EmployeeUpgradeDataSO upgradeData = GetUpgradeData(type);

        bool isMaxLevel = level >= employeeData.MaxLevel;

        int cost = 0;

        if (upgradeData != null && !isMaxLevel) cost = upgradeData.GetCosts(level);

        bool canAction = !isMaxLevel && upgradeData != null &&
            StockManager.CanConsumeCurrency(cost);

        return new StaffInfoUIData
        {
            type = type,

            roleIcon = employeeData.RoleIcon,
            staffName = employeeData.DisplayName,

            level = level,
            maxLevel = employeeData.MaxLevel,

            level1Skill = employeeData.GetSkill(1),
            level3Skill = employeeData.GetSkill(3),
            level5Skill = employeeData.GetSkill(5),

            nextLevelText = isMaxLevel
                ? "최대 레벨입니다." : $"다음 레벨: Lv.{level + 1}",

            nextLevelEffect = isMaxLevel
                ? string.Empty : employeeData.GetLevelEffect(level + 1),

            cost = cost,

            isMaxLevel = isMaxLevel,
            canAction = canAction
        };
    }

    /// <summary>
    /// 직원 모집 또는 강화를 실행합니다.
    /// 현재 레벨이 0이면 모집,
    /// 1 이상이면 다음 레벨 강화가 됩니다.
    /// </summary>
    public bool TryRecruitOrUpgrade(EmployeeType type)
    {
        EmployeeUpgradeDataSO upgradeData = GetUpgradeData(type);

        if (upgradeData == null)
        {
            Debug.LogWarning($"EmployeeUpgradeData가 없습니다. EmployeeType: {type}");

            return false;
        }

        return Upgrade.TryUpgrade(upgradeData);
    }

    /// <summary>
    /// 해당 직원의 UpgradeData를 조회합니다.
    /// </summary>
    private EmployeeUpgradeDataSO GetUpgradeData(EmployeeType type)
    {
        if (!EmployeeDataDB.TryGetData(type, out EmployeeDataSO employeeData))
        {
            Debug.LogWarning($"EmployeeData가 없습니다: {type}");
            return null;
        }

        UpgradeDataSO upgradeData =
            UpgradeDataDB.GetData(employeeData.Id);

        return upgradeData as EmployeeUpgradeDataSO;
    }
}