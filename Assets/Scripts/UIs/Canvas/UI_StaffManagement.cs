using System.Collections;
using UnityEngine;

// Staff Management Canvas의 화면 제어만 담당합니다.
// 직원 데이터 조회 및 모집/강화 로직은 StaffManagementService가 담당합니다.
public sealed class UI_StaffManagement : UI_Base
{
    private enum HubStateButtons
    {
        ExitButton,
        DinerInteriorButton,
        StaffManagerButton
    }

    [SerializeField] private UI_StaffListPanel staffListPanel;

    [SerializeField] private UI_StaffInfoPanel staffInfoPanel;

    private readonly StaffManagementService staffService =
        new StaffManagementService();

    private EmployeeType selectedType = EmployeeType.Count;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

        GetUI<UI_HubStateButton>(
            (int)HubStateButtons.ExitButton)?.Init(Owner);

        GetUI<UI_HubStateButton>(
            (int)HubStateButtons.DinerInteriorButton)?.Init(Owner);

        GetUI<UI_HubStateButton>(
            (int)HubStateButtons.StaffManagerButton)?.Init(Owner);

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

    /// <summary>
    /// 직원 카드를 선택했을 때 호출됩니다.
    /// </summary>
    private void OnSelectStaff(EmployeeType type)
    {
        selectedType = type;

        StaffInfoUIData data = staffService.CreateInfoData(type);

        if (data != null)
            staffInfoPanel.Show(data);
    }

    /// <summary>
    /// 모집/강화 버튼 클릭 시 호출됩니다.
    /// 실제 처리는 StaffManagementService가 담당합니다.
    /// </summary>
    private void OnClickRecruitOrUpgrade(EmployeeType type)
    {
        if (!staffService.TryRecruitOrUpgrade(type)) return;

        RefreshAll();
    }

    /// <summary>
    /// 직원 목록과 선택된 직원 정보를 갱신합니다.
    /// </summary>
    private void RefreshAll()
    {
        staffListPanel.ShowCards(
            staffService.CreateCardDataList()
        );

        if (selectedType == EmployeeType.Count) return;

        StaffInfoUIData data = staffService.CreateInfoData(selectedType);

        if (data != null) staffInfoPanel.Show(data);
    }
}