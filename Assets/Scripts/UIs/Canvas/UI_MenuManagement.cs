using System.Collections;
using UnityEngine;

public sealed class UI_MenuManagement : UI_Base
{
    private UI_SelectMenuPanel selectMenuPanel;
    private UI_MenuSlidePanel menuSlidePanel;
    private UI_MenuVisualizer menuVisualizer;
    private UI_MenuUpgradePanel menuUpgradePanel;
    private UI_DayVisual dayVisual;

    private enum GameObjects
    {
        ExitButton,
        UI_SelectMenuPanel,
        UI_MenuSlidePanel,
        UI_MenuVisualizer,
        UI_MenuUpgradePanel,
        UI_DayVisual
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetUI<GameObject>((int)GameObjects.ExitButton)?
            .GetComponent<UI_HubStateButton>()?.Init(Owner);

        selectMenuPanel = GetUI<GameObject>((int)GameObjects.UI_SelectMenuPanel)?
            .GetComponent<UI_SelectMenuPanel>();
        menuSlidePanel = GetUI<GameObject>((int)GameObjects.UI_MenuSlidePanel)?
            .GetComponent<UI_MenuSlidePanel>();
        menuVisualizer = GetUI<GameObject>((int)GameObjects.UI_MenuVisualizer)?
            .GetComponent<UI_MenuVisualizer>();
        menuUpgradePanel = GetUI<GameObject>((int)GameObjects.UI_MenuUpgradePanel)?
            .GetComponent<UI_MenuUpgradePanel>();
        dayVisual = GetUI<GameObject>((int)GameObjects.UI_DayVisual)?
            .GetComponent<UI_DayVisual>();

        selectMenuPanel?.SetCanDeselect(true);
        selectMenuPanel?.Init(Owner);
        menuSlidePanel?.Init();
        menuVisualizer?.SetUpgradePanel(menuUpgradePanel);
        menuVisualizer?.SetData(DishType.None);
        menuUpgradePanel?.Hide();
    }

    protected override IEnumerator OnShow()
    {
        selectMenuPanel?.Refresh();
        menuSlidePanel?.Refresh();
        menuVisualizer?.SetData(DishType.None);
        menuUpgradePanel?.Hide();
        dayVisual?.Refresh();

        yield break;
    }

    protected override IEnumerator OnHide()
    {
        yield break;
    }
}
