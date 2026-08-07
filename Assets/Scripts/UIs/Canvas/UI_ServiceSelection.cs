using System.Collections;
using UnityEngine;

public sealed class UI_ServiceSelection : UI_Base
{
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
        StartServiceButton?.AddUIEvent(_ => GameManager.Instance.Scene.ChangeScene(SceneType.Service), UI_EventHandler.UIEvent.LClick);

        UI_SelectMenuPanel selectMenuPanel =
            GetGameObject((int)GameObjects.UI_SelectMenuPanel)?
                .GetComponent<UI_SelectMenuPanel>();
        selectMenuPanel?.SetCanDeselect(false);
        selectMenuPanel?.Init(Owner);
    }

    private void Start()
    {
        // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
    }

    protected override IEnumerator OnShow()
    {
        // 화면을 표시할 때 갱신할 값과 등장 연출을 작성합니다.
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        // 화면을 숨기기 전 정리할 값과 퇴장 연출을 작성합니다.
        yield break;
    }
}
