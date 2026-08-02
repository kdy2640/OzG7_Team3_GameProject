using System.Collections;
using UnityEngine;

public sealed class UI_HarvestSelection : UI_Base
{
    private enum GameObjects
    {
        To_HubView
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetGameObject((int)GameObjects.To_HubView)?
            .GetComponent<UI_HubStateButton>()?
            .Init(Owner);
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
