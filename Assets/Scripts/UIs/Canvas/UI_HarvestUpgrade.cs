using System.Collections;
using TMPro;
using UnityEngine;

public sealed class UI_HarvestUpgrade : UI_Base
{
    private enum GameObjects
    {
        ExitButton
    }
    private enum Texts
    {
        UI_TempText
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetGameObject((int)GameObjects.ExitButton)?
            .GetComponent<UI_HubStateButton>()?
            .Init(Owner);
        GetUI<TextMeshProUGUI>((int)Texts.UI_TempText).text = "임시";
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
