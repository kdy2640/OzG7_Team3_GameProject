using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 Canvas 전용 Controller입니다. 게임 데이터 조회와 모집/강화 실행은 여기만 담당합니다.
public sealed class UI_StaffManagement : UI_Base
{
    private enum HubStateButtons { ExitButton, DinerInteriorButton, StaffManagerButton }

    [SerializeField] private UI_StaffListPanel staffListPanel;
    [SerializeField] private UI_StaffInfoPanel staffInfoPanel;

    private static Dictionary<EmployeeType, EmployeeUpgradeDataSO> upgradeDataMap;
    private EmployeeType selectedType = EmployeeType.Count;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?.Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.DinerInteriorButton)?.Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.StaffManagerButton)?.Init(Owner);

        staffListPanel.Initialize(OnSelectStaff);
        staffInfoPanel.Initialize(OnClickRecruitOrUpgrade);
    }

    protected override IEnumerator OnShow()
    {
        RefreshAll();
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        staffListPanel.HideAllCards();
        yield break;
    }

    private void OnSelectStaff(EmployeeType type)
    {
        selectedType = type;
        staffInfoPanel.Show(CreateInfoData(type));
    }

    private void OnClickRecruitOrUpgrade(EmployeeType type)
    {
        EmployeeUpgradeDataSO upgradeData = GetUpgradeData(type);
        if (upgradeData != null && GameManager.Instance.Upgrade.TryUpgrade(upgradeData))
            RefreshAll();
    }

    private void RefreshAll()
    {
        staffListPanel.ShowCards(CreateCardDataList());
        if (selectedType != EmployeeType.Count)
            staffInfoPanel.Show(CreateInfoData(selectedType));
    }

    private List<StaffCardUIData> CreateCardDataList()
    {
        var result = new List<StaffCardUIData>();
        foreach (EmployeeType type in System.Enum.GetValues(typeof(EmployeeType)))
        {
            if (type == EmployeeType.Count || !EmployeeDataDB.TryGetData(type, out EmployeeDataSO data)) continue;
            int level = GameManager.Instance.Upgrade.RuntimeStat.Employee.GetLevel(type);
            EmployeeUpgradeDataSO upgrade = GetUpgradeData(type);
            bool canPay = upgrade != null && GameManager.Instance.StockManager.CanConsumeCurrency(upgrade.GetCosts(level));
            StaffCardState state = level == 0 ? (canPay ? StaffCardState.CanRecruit : StaffCardState.Locked)
                : level >= data.MaxLevel ? StaffCardState.Normal
                : canPay ? StaffCardState.CanUpgrade : StaffCardState.Normal;
            result.Add(new StaffCardUIData { type = type, level = level, state = state });
        }
        return result;
    }

    private StaffInfoUIData CreateInfoData(EmployeeType type)
    {
        if (!EmployeeDataDB.TryGetData(type, out EmployeeDataSO data))
        {
            Debug.LogWarning($"EmployeeData가 없습니다: {type}");
            return null;
        }
        int level = GameManager.Instance.Upgrade.RuntimeStat.Employee.GetLevel(type);
        EmployeeUpgradeDataSO upgrade = GetUpgradeData(type);
        bool max = level >= data.MaxLevel;
        int cost = upgrade == null ? 0 : upgrade.GetCosts(level);
        return new StaffInfoUIData
        {
            type = type,
            roleIcon = data.RoleIcon,
            staffName = data.DisplayName,
            level = level,
            maxLevel = data.MaxLevel,
            level1Skill = data.GetSkill(1),
            level3Skill = data.GetSkill(3),
            level5Skill = data.GetSkill(5),
            nextLevelText = max ? "최대 레벨입니다." : $"다음 레벨: Lv.{level + 1}",
            nextLevelEffect = max ? "" : data.GetLevelEffect(level + 1),
            cost = cost,
            isMaxLevel = max,
            canAction = !max && upgrade != null && GameManager.Instance.StockManager.CanConsumeCurrency(cost)
        };
    }

    private static EmployeeUpgradeDataSO GetUpgradeData(EmployeeType type)
    {
        if (upgradeDataMap == null)
        {
            upgradeDataMap = new Dictionary<EmployeeType, EmployeeUpgradeDataSO>();
            foreach (EmployeeUpgradeDataSO data in Resources.LoadAll<EmployeeUpgradeDataSO>("SOs/UpgradeDatas/Employee"))
                if (data != null) upgradeDataMap[data.TargetEmployee] = data;
        }
        upgradeDataMap.TryGetValue(type, out EmployeeUpgradeDataSO result);
        return result;
    }
}
