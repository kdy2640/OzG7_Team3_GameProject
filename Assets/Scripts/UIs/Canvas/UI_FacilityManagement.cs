using System.Collections;
using UnityEngine;

public sealed class UI_FacilityManagement : UI_Base
{
    private enum HubStateButtons
    {
        ExitButton,
        DinerInteriorButton,
        StaffManagerButton
    }

    private FacilityCollection facilityCollection;
    private FacilityDetailPanel detailPanel;

    private PanelAnimator[] panelAnimators;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?
            .Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.DinerInteriorButton)?
            .Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.StaffManagerButton)?
            .Init(Owner);

        detailPanel = GetComponentInChildren<FacilityDetailPanel>(true);

        // 애니메이션 실행 순서를 코드에서 명시적으로 정의 (배열 순서 = 연출 순서)
        panelAnimators = new[]
        {
            GetPanelAnimator(detailPanel)
        };
    }

    protected override IEnumerator OnShow()
    {
        facilityCollection = FindFirstObjectByType<FacilityCollection>();

        detailPanel.Initialize(facilityCollection);
        facilityCollection.FacilitySelected += detailPanel.ShowFacility;

        // 패널 등장 애니메이션 일괄 재생
        PlayPanelAnimations();

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        if (facilityCollection != null)
        {
            facilityCollection.FacilitySelected -= detailPanel.ShowFacility;
        }

        yield break;
    }

    private void PlayPanelAnimations()
    {
        if (panelAnimators == null) return;

        foreach (PanelAnimator animator in panelAnimators)
        {
            if (animator == null) continue;
            if (!animator.gameObject.activeInHierarchy) continue;

            animator.Show();
        }
    }

    private PanelAnimator GetPanelAnimator(Component target)
    {
        if (target == null) return null;

        PanelAnimator animator = target.GetComponent<PanelAnimator>();

        if (animator == null)
        {
            Debug.LogWarning($"[{GetType().Name}] '{target.name}'에 PanelAnimator가 없습니다.", target);
        }

        return animator;
    }
}