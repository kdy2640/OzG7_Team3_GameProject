using System.Collections;
using UnityEngine;

public sealed class UI_MenuManagement : UI_Base
{
    private UI_SelectedMenuPanel selectedMenuPanel; 
    private UI_MenuSlidePanel menuSlidePanel;
    private UI_MenuVisualizer menuVisualizer;

    private enum GameObjects
    {
        ExitButton,
        UI_SelectedMenuPanel,
        UI_MenuSlidePanel,
        UI_MenuVisualizer
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetUI<GameObject>((int)GameObjects.ExitButton)?
            .GetComponent<UI_HubStateButton>()?.Init(Owner);

        selectedMenuPanel = GetUI<GameObject>((int)GameObjects.UI_SelectedMenuPanel)?
            .GetComponent<UI_SelectedMenuPanel>(); 
        menuSlidePanel = GetUI<GameObject>((int)GameObjects.UI_MenuSlidePanel)?
            .GetComponent<UI_MenuSlidePanel>();
        menuVisualizer = GetUI<GameObject>((int)GameObjects.UI_MenuVisualizer)?
            .GetComponent<UI_MenuVisualizer>();

        selectedMenuPanel?.Refresh();
        menuSlidePanel?.Init();
        menuVisualizer?.SetData(DishType.MeatOnigiri);
    }

    protected override IEnumerator OnShow()
    {
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        yield break;
    }
}
