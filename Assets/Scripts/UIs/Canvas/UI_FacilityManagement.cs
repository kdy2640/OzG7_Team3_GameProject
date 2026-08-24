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

        PlayPanelAnimations();

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        facilityCollection.FacilitySelected -= detailPanel.ShowFacility;

        yield break;
    }
}
