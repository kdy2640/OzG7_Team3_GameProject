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

    private enum PanelAnimators
    {
        Header,
        UI_CommonExitPanel
    }

    private FacilityCollection facilityCollection;
    private FacilityDetailPanel detailPanel;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<PanelAnimator>(typeof(PanelAnimators));

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
        facilityCollection.FacilitySelected += OnFacilitySelected;

        GetUI<PanelAnimator>((int)PanelAnimators.Header).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        if (facilityCollection != null)
        {
            facilityCollection.FacilitySelected -= OnFacilitySelected;
        }

        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.Header).Hide();
    }

    private void OnFacilitySelected(FacilityType facilityType)
    {
        StartCoroutine(detailPanel.ShowFacility(facilityType));
    }
}
