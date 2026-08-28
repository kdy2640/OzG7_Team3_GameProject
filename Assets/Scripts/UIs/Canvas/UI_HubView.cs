using System.Collections;
using UnityEngine;

public sealed class UI_HubView : UI_Base
{
    private UI_HubMenuPanel hubMenuPanel;
    private UI_MarketVisualPanel marketVisualPanel;
    private UI_PhaseButtonPanel phaseButtonPanel;
    private UI_DayVisual dayVisual;

    private enum GameObjects
    {
        UI_HubMenuPanel,
        UI_MarketVisualPanel,
        UI_PhasePanel,
        UI_DayVisual
    }

    private enum PanelAnimators
    {
        UI_DayVisual,
        UI_CommonSettingsPanel,
        UI_HubMenuPanel,
        UI_MarketVisualPanel,
        UI_PhasePanel
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<PanelAnimator>(typeof(PanelAnimators));

        hubMenuPanel = GetGameObject((int)GameObjects.UI_HubMenuPanel)?
            .GetComponent<UI_HubMenuPanel>();
        hubMenuPanel.Init(Owner);

        marketVisualPanel = GetGameObject((int)GameObjects.UI_MarketVisualPanel)?
            .GetComponent<UI_MarketVisualPanel>();
        marketVisualPanel.Init(Owner);
        marketVisualPanel.Refresh();

        phaseButtonPanel = GetGameObject((int)GameObjects.UI_PhasePanel)?
            .GetComponent<UI_PhaseButtonPanel>();
        phaseButtonPanel.Init(Owner);

        dayVisual = GetGameObject((int)GameObjects.UI_DayVisual)?
            .GetComponent<UI_DayVisual>();
        dayVisual.Refresh();
    }

    private void Start()
    {
        // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
    }

    protected override IEnumerator OnShow()
    {
        marketVisualPanel?.Refresh();
        phaseButtonPanel?.Refresh();
        dayVisual?.Refresh();

        GetUI<PanelAnimator>((int)PanelAnimators.UI_DayVisual).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonSettingsPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_HubMenuPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MarketVisualPanel).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_PhasePanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        GetUI<PanelAnimator>((int)PanelAnimators.UI_PhasePanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MarketVisualPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_HubMenuPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonSettingsPanel).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_DayVisual).Hide();
    }
}
