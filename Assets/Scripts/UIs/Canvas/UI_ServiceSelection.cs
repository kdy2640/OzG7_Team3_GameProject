using System.Collections;
using UnityEngine;

public sealed class UI_ServiceSelection : UI_Base
{
    private UI_SelectMenuPanel selectMenuPanel;
    private UI_MarketVisualPanel marketVisualPanel;

    private enum GameObjects
    {
        UI_StartServiceButton,
        UI_SelectMenuPanel
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    private enum PanelAnimators
    {
        Header,
        TopLeft,
        UI_CommonExitPanel,
        UI_SelectMenuPanel,
        UI_MareketVisualPanel
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

    }

    protected override IEnumerator OnShow()
    {
        selectMenuPanel?.Refresh();
        marketVisualPanel?.Refresh();

        GetUI<PanelAnimator>((int)PanelAnimators.Header).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.TopLeft).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_SelectMenuPanel).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_MareketVisualPanel).Show();
    }

    protected override IEnumerator OnHide()
    {
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MareketVisualPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_SelectMenuPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.TopLeft).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.Header).Hide();
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
