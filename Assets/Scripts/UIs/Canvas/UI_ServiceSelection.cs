using System.Collections;
using UnityEngine;

public sealed class UI_ServiceSelection : UI_Base
{
    private UI_SelectMenuPanel selectMenuPanel;
    private UI_MarketVisualPanel marketVisualPanel;

    private PanelAnimator[] panelAnimators;

    private enum GameObjects
    {
        UI_StartServiceButton,
        UI_SelectMenuPanel
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?.Init(Owner);

        UI_EventHandler startServiceButton =
            GetGameObject((int)GameObjects.UI_StartServiceButton)?.GetComponent<UI_EventHandler>();

        startServiceButton?.AddUIEvent(_ => StartService(),UI_EventHandler.UIEvent.LClick);

        selectMenuPanel = GetGameObject((int)GameObjects.UI_SelectMenuPanel)?
                .GetComponent<UI_SelectMenuPanel>();

        selectMenuPanel?.SetCanDeselect(false);
        selectMenuPanel?.Init(Owner);

        marketVisualPanel = GetComponentInChildren<UI_MarketVisualPanel>(true);

        // 하위 오브젝트에 붙어 있는 PanelAnimator 자동 수집
        panelAnimators = GetComponentsInChildren<PanelAnimator>(true);
    }

    protected override IEnumerator OnShow()
    {
        selectMenuPanel?.Refresh();
        marketVisualPanel?.Refresh();

        PlayPanelAnimations();

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        yield break;
    }

    private void PlayPanelAnimations()
    {
        if (panelAnimators == null) return;

        foreach (PanelAnimator animator in panelAnimators)
        {
            if (animator == null) continue;

            if (!animator.PlayOnParentShow) continue;

            if (!animator.gameObject.activeInHierarchy) continue;

            animator.Show();
        }
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