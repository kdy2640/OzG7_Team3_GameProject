using System.Collections;

public sealed class UI_HarvestUpgrade : UI_Base
{
    private enum HubStateButtons
    {
        ExitButton
    }

    private enum PanelAnimators
    {
        UpgradeListPanel,
        PreviewPanel,
        UI_CommonExitPanel
    }

    private HarvestUpgradeListPanel upgradeListPanel;
    private HarvestUpgradePreviewPanel previewPanel;
    private HarvestUpgradeDetailPanel detailPanel;

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<PanelAnimator>(typeof(PanelAnimators));
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

        GetUI<PanelAnimator>((int)PanelAnimators.UpgradeListPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.PreviewPanel).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        upgradeListPanel?.ClearSelection();
        previewPanel?.ClearHighlight();
        detailPanel?.ClosePanel();

        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.PreviewPanel).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UpgradeListPanel).Hide();
    }

    private void OnDestroy()
    {
        if (upgradeListPanel != null)
            upgradeListPanel.OnSelected -= ShowDetail;
    }

    private void ShowDetail(HarvestUpgradeType upgradeType)
    {
        StartCoroutine(detailPanel.ShowUpgrade(upgradeType));
    }
}
