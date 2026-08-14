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

        if (facilityCollection == null || detailPanel == null)
        {
            Debug.LogError(
                "[UI_FacilityManagement] FacilityCollection 또는 " +
                "FacilityDetailPanel을 찾을 수 없습니다.",
                this);
            yield break;
        }

        detailPanel.Initialize(facilityCollection);
        facilityCollection.FacilitySelected += detailPanel.ShowFacility;

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        if (facilityCollection != null && detailPanel != null)
        {
            facilityCollection.FacilitySelected -= detailPanel.ShowFacility;
        }

        yield break;
    }
}
