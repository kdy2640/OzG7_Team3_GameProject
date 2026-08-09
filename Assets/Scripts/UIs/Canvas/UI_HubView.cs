using System.Collections;
using UnityEngine;

public sealed class UI_HubView : UI_Base
{
    private UI_MareketVisualPanel marketVisualPanel;

    private enum GameObjects
    {
        UI_ToFacilityManagementButton,
        UI_ToMenuManagementButton,
        UI_ToStoreButton,
        UI_ToLogButton,
        UI_ToHarvestButton,
        UI_ToServiceSelectionButton
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetGameObject((int)GameObjects.UI_ToFacilityManagementButton)?
            .GetComponent<UI_HubStateButton>()?
            .Init(Owner);
        GetGameObject((int)GameObjects.UI_ToMenuManagementButton)?
            .GetComponent<UI_HubStateButton>()?
            .Init(Owner);
        GetGameObject((int)GameObjects.UI_ToHarvestButton)?
            .GetComponent<UI_HubStateButton>()?
            .Init(Owner);
        GetGameObject((int)GameObjects.UI_ToServiceSelectionButton)?
            .GetComponent<UI_HubStateButton>()?
            .Init(Owner);

        marketVisualPanel = GetComponentInChildren<UI_MareketVisualPanel>(true);
        marketVisualPanel?.Refresh();
    }

    private void Start()
    {
        // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
    }

    protected override IEnumerator OnShow()
    {
        marketVisualPanel?.Refresh();
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        // 화면을 숨기기 전 정리할 값과 퇴장 연출을 작성합니다.
        yield break;
    }
}
