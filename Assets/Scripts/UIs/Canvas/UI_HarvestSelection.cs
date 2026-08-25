using System.Collections;
using UnityEngine;

public sealed class UI_HarvestSelection : UI_Base
{
    private enum GameObjects
    {
        UI_HarvestStageListPanel,
        UI_HarvestStageDetailPanel
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    private enum PanelAnimators
    {
        UI_HarvestStageListPanel,
        UI_HarvestStageDetailPanel,
        UI_GroceryViewPanel,
        UI_CommonExitPanel
    }

    private UI_HarvestStageListPanel stageListPanel;
    private UI_HarvestStageDetailPanel stageDetailPanel;

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<PanelAnimator>(typeof(PanelAnimators));

        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?
            .Init(Owner);

        stageListPanel =
            GetGameObject((int)GameObjects.UI_HarvestStageListPanel)?
                .GetComponent<UI_HarvestStageListPanel>();
        stageDetailPanel =
            GetGameObject((int)GameObjects.UI_HarvestStageDetailPanel)?
                .GetComponent<UI_HarvestStageDetailPanel>();

        stageListPanel?.Initialize();
        stageDetailPanel?.Initialize();

        if (stageListPanel != null)
        {
            stageListPanel.OnSelected += ShowStage;
            stageListPanel.Select(StageType.Stage_1);
        }
    }

    protected override IEnumerator OnShow()
    {
        stageDetailPanel?.Refresh();

        GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_HarvestStageListPanel).Show();
        GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_HarvestStageDetailPanel).Show();
        GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_GroceryViewPanel).Show();
        yield return GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_CommonExitPanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_CommonExitPanel).Hide();
        GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_GroceryViewPanel).Hide();
        GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_HarvestStageDetailPanel).Hide();
        yield return GetUI<PanelAnimator>(
            (int)PanelAnimators.UI_HarvestStageListPanel).Hide();
    }

    private void OnDestroy()
    {
        if (stageListPanel != null)
            stageListPanel.OnSelected -= ShowStage;
    }

    private void ShowStage(StageType stageType)
    {
        stageDetailPanel?.Show(stageType);
    }
}
