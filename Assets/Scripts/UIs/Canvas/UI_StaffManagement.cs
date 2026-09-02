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

    private enum PanelAnimators
    {
        Header,
        UI_CommonExitPanel,
        StaffListPanel
    }

    [Header("Panels")]
    [SerializeField] private UI_StaffListPanel staffListPanel;
    [SerializeField] private UI_StaffInfoPanel staffInfoPanel;

    private readonly StaffManagementService staffService = new StaffManagementService();

    private EmployeeType selectedType = EmployeeType.Count;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<PanelAnimator>(typeof(PanelAnimators));

        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?.Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.DinerInteriorButton)?.Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.StaffManagerButton)?.Init(Owner);

        staffListPanel.Initialize(OnSelectStaff);
        staffInfoPanel.Initialize(OnClickRecruitOrUpgrade);

    }

    protected override IEnumerator OnShow()
    {
        selectedType = EmployeeType.Count;
        RefreshAll();
        staffListPanel.SelectFirst();

        GetUI<PanelAnimator>((int)PanelAnimators.Header).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.StaffListPanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        Coroutine staffInfoHide = StartCoroutine(staffInfoPanel.Hide());

        GetUI<PanelAnimator>((int)PanelAnimators.StaffListPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.Header).Hide();
        yield return staffInfoHide;
        staffListPanel.HideAllCards();
    }

    private void OnSelectStaff(EmployeeType type)
    {
        GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Hub_Select);
        selectedType = type;

        StartCoroutine(staffInfoPanel.Show(type));
    }

    private void OnClickRecruitOrUpgrade(EmployeeType type)
    {
        if (!staffService.TryRecruitOrUpgrade(type)) return;

        RefreshAll();
    }

    private void RefreshAll()
    {
        staffListPanel.ShowCards();

        if (selectedType != EmployeeType.Count)
            StartCoroutine(staffInfoPanel.Show(selectedType));
    }
}
