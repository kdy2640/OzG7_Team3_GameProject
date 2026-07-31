using System.Collections;

public sealed class UI_HarvestUpgrade : UI_Base
{
    private enum Buttons
    {
        To_HubView
    }

    protected override void OnInit()
    {
        Bind<UI_HubStateButton>(typeof(Buttons));
        GetUI<UI_HubStateButton>((int)Buttons.To_HubView)?.Init(Owner);
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
