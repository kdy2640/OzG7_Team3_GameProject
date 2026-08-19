using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class UI_DayStart : UI_Base
{
    private UI_DayVisualPanel dayVisualPanel;
    private UI_FestivalPanel festivalPanel;
    private UI_DishDetailPanel dishDetailPanel;

    private enum GameObjects
    {
        UI_DayVisualPanel,
        UI_FestivalPanel,
        UI_DishDetailPanel
    }

    private enum HubStateButtons
    {
        UI_ToHubButton
    }

    private enum Buttons
    {
        UI_FestivalStartButton
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<Button>(typeof(Buttons));

        dayVisualPanel = GetGameObject((int)GameObjects.UI_DayVisualPanel)?
            .GetComponent<UI_DayVisualPanel>();
        festivalPanel = GetGameObject((int)GameObjects.UI_FestivalPanel)?
            .GetComponent<UI_FestivalPanel>();
        dishDetailPanel = GetGameObject((int)GameObjects.UI_DishDetailPanel)?
            .GetComponent<UI_DishDetailPanel>();

        dayVisualPanel?.Init();
        festivalPanel?.Init(
            dishDetailPanel,
            GetButton((int)Buttons.UI_FestivalStartButton));

        GetUI<UI_HubStateButton>((int)HubStateButtons.UI_ToHubButton)?
            .Init(Owner);
    }

    private void Start()
    {
        // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
    }

    protected override IEnumerator OnShow()
    {
        if (dayVisualPanel == null || GameManager.Instance?.Market == null)
        {
            Debug.LogError($"[{nameof(UI_DayStart)}] 날짜 연출에 필요한 참조를 찾을 수 없습니다.", this);
            yield break;
        }

        festivalPanel?.Refresh();

        int currentBusinessDay = GameManager.Instance.Market.MarketData.CurrentBusinessDay;
        yield return dayVisualPanel.SyncAndPlay(currentBusinessDay);

        // 화면을 표시할 때 갱신할 값과 등장 연출을 작성합니다.
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        // 화면을 숨기기 전 정리할 값과 퇴장 연출을 작성합니다.
        yield break;
    }
}
