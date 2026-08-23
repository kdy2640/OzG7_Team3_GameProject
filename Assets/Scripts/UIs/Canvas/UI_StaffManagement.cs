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

    [Header("Entrance Animations")]
    // OnShow 시점에 동시에 등장할 패널들
    [SerializeField] private PanelAnimator[] defaultEntranceAnimators;

    // 클릭(선택) 시점에 별도로 연출될 인포 패널 애니메이터
    [SerializeField] private PanelAnimator infoPanelAnimator;

    private readonly StaffManagementService staffService =
        new StaffManagementService();

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

        if (defaultEntranceAnimators != null)
        {
            foreach (var animator in defaultEntranceAnimators)
            {
                animator?.Show();
            }
        }

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
        staffInfoPanel.Show(type);
        
        //스태프 선택시 infoPanel만 연출 실행
        if (isNewSelection && infoPanelAnimator != null)
        {
            infoPanelAnimator.Show();
        }
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