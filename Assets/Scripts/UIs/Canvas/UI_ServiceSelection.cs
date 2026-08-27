using System.Collections;
using UnityEngine;

public sealed class UI_ServiceSelection : UI_Base
{
    private UI_SelectMenuPanel selectMenuPanel;
    private UI_MarketVisualPanel marketVisualPanel;
    private UI_DayVisual dayVisual;

    private enum GameObjects
    {
        UI_StartServiceButton,
        UI_SelectMenuPanel,
        UI_DayVisual
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    private enum PanelAnimators
    {
        UI_DayVisual,
        TopLeft,
        UI_CommonExitPanel,
        UI_SelectMenuPanel,
        UI_MarketVisualPanel
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<PanelAnimator>(typeof(PanelAnimators));

        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?.Init(Owner);

        UI_EventHandler startServiceButton =
            GetGameObject((int)GameObjects.UI_StartServiceButton)?.GetComponent<UI_EventHandler>();

        startServiceButton?.AddUIEvent(_ => StartService(),UI_EventHandler.UIEvent.LClick);

        selectMenuPanel = GetGameObject((int)GameObjects.UI_SelectMenuPanel)?
                .GetComponent<UI_SelectMenuPanel>();

        selectMenuPanel?.SetCanDeselect(false);
        selectMenuPanel?.Init(Owner);

        marketVisualPanel = GetComponentInChildren<UI_MarketVisualPanel>(true);

        dayVisual = GetGameObject((int)GameObjects.UI_DayVisual)
            .GetComponent<UI_DayVisual>();
        dayVisual.Refresh();

    }

    protected override IEnumerator OnShow()
    {
        selectMenuPanel?.Refresh();
        marketVisualPanel?.Refresh();
        dayVisual.Refresh();

        GetUI<PanelAnimator>((int)PanelAnimators.UI_DayVisual).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.TopLeft).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_SelectMenuPanel).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_MarketVisualPanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MarketVisualPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_SelectMenuPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.TopLeft).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_DayVisual).Hide();
    }

    private void StartService()
    {
        if (GameManager.Instance.Market.MarketData.SelectedDishes.Count == 0)
        {
            GameManager.Instance.Utility.Toast.Show("메뉴를 선택해 주세요");
            return;
        }

        GameManager.Instance.Scene.ChangeScene(SceneType.Service);
    }
}
