public sealed class UI_HubMenuPanel : UI_Base
{
    private enum HubStateButtons
    {
        UI_ToFacilityManagementButton,
        UI_ToMenuManagementButton,
        UI_ToHarvestUpgradeButton
    }

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

        GetUI<UI_HubStateButton>((int)HubStateButtons.UI_ToFacilityManagementButton)?
            .Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.UI_ToMenuManagementButton)?
            .Init(Owner);
        GetUI<UI_HubStateButton>((int)HubStateButtons.UI_ToHarvestUpgradeButton)?
            .Init(Owner);
    }
}
