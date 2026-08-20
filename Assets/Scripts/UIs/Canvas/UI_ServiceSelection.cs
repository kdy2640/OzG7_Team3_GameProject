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

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton)?
            .Init(Owner);
        UI_EventHandler StartServiceButton = GetGameObject((int)GameObjects.UI_StartServiceButton)?.GetComponent<UI_EventHandler>();
        StartServiceButton?.AddUIEvent(_ => StartService(), UI_EventHandler.UIEvent.LClick);

        selectMenuPanel =
            GetGameObject((int)GameObjects.UI_SelectMenuPanel)?
                .GetComponent<UI_SelectMenuPanel>();
        selectMenuPanel?.SetCanDeselect(false);
        selectMenuPanel?.Init(Owner);

        marketVisualPanel = GetComponentInChildren<UI_MarketVisualPanel>(true);
        marketVisualPanel?.Refresh();
    }

    private void StartService()
    {
        if (GameManager.Instance.Market.MarketData.SelectedDishes.Count == 0)
        {
            GameManager.Instance.Utility.Toast.Show("메뉴를 선택홰 주세요");
            return;
        }

        GameManager.Instance.Scene.ChangeScene(SceneType.Service);
    }

    private void Start()
    {
        // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
    }

    protected override IEnumerator OnShow()
    {
        selectMenuPanel?.Refresh();
        marketVisualPanel?.Refresh();
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        // 화면을 숨기기 전 정리할 값과 퇴장 연출을 작성합니다.
        yield break;
    }
}
