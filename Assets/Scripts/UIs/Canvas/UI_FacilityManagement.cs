using DG.Tweening;
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
    //애니메이터 저장용 배열
    [Header("Panel Animations")]
    [SerializeField] private PanelAnimator[] defaultEntranceAnimators;

    private FacilityCollection facilityCollection;
    private FacilityDetailPanel detailPanel;


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

    }

    protected override IEnumerator OnShow()
    {
        facilityCollection = FindFirstObjectByType<FacilityCollection>(); 

        detailPanel.Initialize(facilityCollection);
        facilityCollection.FacilitySelected += detailPanel.ShowFacility;

        // 상시 패널 등장 연출
        if (defaultEntranceAnimators != null)
        {
            foreach (var animator in defaultEntranceAnimators)
            {
                if (animator != null && animator.gameObject.activeInHierarchy)
                {
                    animator.Show();
                }
            }
        }

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        facilityCollection.FacilitySelected -= detailPanel.ShowFacility;

        yield break;
    }
}
