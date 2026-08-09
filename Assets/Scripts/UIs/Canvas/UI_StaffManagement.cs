using System.Collections;
using UnityEngine;

public sealed class UI_StaffManagement : UI_Base
{
    [SerializeField] private UI_StaffInfoPanel staffInfoPanel;
    [SerializeField] private UI_StaffDevelopCard[] staffCards;
    private enum HubStateButtons
    {
        ExitButton,
        DinerInteriorButton,
        StaffManagerButton
    }

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?
            .Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.DinerInteriorButton)?
            .Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.StaffManagerButton)?
            .Init(Owner);

        foreach (UI_StaffDevelopCard card in staffCards)
            card.Init(staffInfoPanel);
    }

    protected override IEnumerator OnShow()
    {
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        yield break;
    }
}
