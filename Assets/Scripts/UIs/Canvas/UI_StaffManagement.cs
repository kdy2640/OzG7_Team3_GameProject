using System.Collections;
using UnityEngine;

// Staff Management Canvas의 화면 제어와 패널 간 이벤트 연결을 담당합니다.
public sealed class UI_StaffManagement : UI_Base
{
    private enum HubStateButtons
    {
        ExitButton,
        DinerInteriorButton,
        StaffManagerButton
    }
    [Header("Panels")]
    [SerializeField] private UI_StaffListPanel staffListPanel;
    [SerializeField] private UI_StaffInfoPanel staffInfoPanel;

    private PanelAnimator[] panelAnimators;

    private readonly StaffManagementService staffService = new StaffManagementService();

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

        PlayPanelAnimations();

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        staffListPanel.HideAllCards();
        yield break;
    }

    private void OnSelectStaff(EmployeeType type)
    {

        bool isNewSelection = (selectedType != type);

        selectedType = type;
        staffInfoPanel.Show(type, isNewSelection);

    }

    private void OnClickRecruitOrUpgrade(EmployeeType type)
    {
        if (!staffService.TryRecruitOrUpgrade(type)) return;

        RefreshAll();
    }

    private void RefreshAll()
    {
        staffListPanel.ShowCards();

        if (selectedType != EmployeeType.Count) staffInfoPanel.Show(selectedType);
    }
}