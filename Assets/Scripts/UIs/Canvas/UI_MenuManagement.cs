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

    private enum PanelAnimators
    {
        UI_DayVisual,
        UI_CommonExitPanel,
        UI_SelectMenuPanel,
        UI_MenuSlidePanel,
        UI_MenuVisualizer
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<PanelAnimator>(typeof(PanelAnimators));
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

        GetUI<PanelAnimator>((int)PanelAnimators.UI_DayVisual).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_SelectMenuPanel).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MenuSlidePanel).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_MenuVisualizer).Show();
    }
    protected override IEnumerator OnHide()
    {
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MenuVisualizer).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_MenuSlidePanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_SelectMenuPanel).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_CommonExitPanel).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.UI_DayVisual).Hide();
    }
}
