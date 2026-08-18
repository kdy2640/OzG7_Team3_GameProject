using System.Collections;

public sealed class UI_HarvestUpgrade : UI_Base
{
    private enum HubStateButtons
    {
        ExitButton
    }

    private HarvestUpgradeListPanel upgradeListPanel;
    private HarvestUpgradePreviewPanel previewPanel;
    private HarvestUpgradeDetailPanel detailPanel;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?.Init(Owner);

        upgradeListPanel = GetComponentInChildren<HarvestUpgradeListPanel>(true);
        previewPanel = GetComponentInChildren<HarvestUpgradePreviewPanel>(true);
        detailPanel = GetComponentInChildren<HarvestUpgradeDetailPanel>(true);

        if (upgradeListPanel != null)
        {
            upgradeListPanel.OnSelected += ShowDetail;
            previewPanel?.Initialize(upgradeListPanel);
        }
    }

    protected override IEnumerator OnShow()
    {
        upgradeListPanel?.ClearSelection();
        previewPanel?.ClearHighlight();
        detailPanel?.ClosePanel();
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        upgradeListPanel?.ClearSelection();
        previewPanel?.ClearHighlight();
        detailPanel?.ClosePanel();
        yield break;
    }

    private void OnDestroy()
    {
        if (upgradeListPanel != null)
            upgradeListPanel.OnSelected -= ShowDetail;
    }

    private void ShowDetail(HarvestUpgradeType upgradeType)
    {
        detailPanel?.ShowUpgrade(upgradeType);
    }
}
