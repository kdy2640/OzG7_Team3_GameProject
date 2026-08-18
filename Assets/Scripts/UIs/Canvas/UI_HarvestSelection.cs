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

    private UI_HarvestStageListPanel stageListPanel;
    private UI_HarvestStageDetailPanel stageDetailPanel;

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

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
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        yield break;
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
