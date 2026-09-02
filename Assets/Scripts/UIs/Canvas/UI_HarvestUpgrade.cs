using System.Collections;
using UnityEngine;

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
        upgradeListPanel.SelectFirst();

        Coroutine listShow = StartCoroutine(
            GetUI<PanelAnimator>((int)PanelAnimators.UpgradeListPanel).Show());
        Coroutine previewShow = StartCoroutine(
            GetUI<PanelAnimator>((int)PanelAnimators.PreviewPanel).Show());
        Coroutine exitShow = StartCoroutine(
            GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show());

        yield return listShow;
        yield return previewShow;
        yield return exitShow;
    }

    protected override IEnumerator OnHide()
    {
        upgradeListPanel?.ClearSelection();
        previewPanel?.ClearHighlight();

        Coroutine detailHide = StartCoroutine(detailPanel.HidePanel());
        Coroutine exitHide = StartCoroutine(
            GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide());
        Coroutine previewHide = StartCoroutine(
            GetUI<PanelAnimator>((int)PanelAnimators.PreviewPanel).Hide());
        Coroutine listHide = StartCoroutine(
            GetUI<PanelAnimator>((int)PanelAnimators.UpgradeListPanel).Hide());

        yield return detailHide;
        yield return exitHide;
        yield return previewHide;
        yield return listHide;
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
