using System.Collections;
using UnityEngine;

public sealed class UI_RankUpPanel : UI_Base
{
    private UI_RastaurantLevel rastaurantLevel;
    private UI_NewDishList newDishList;
    private UI_NewIngredientList newIngredientList;
    private UI_NewFunctionList newFunctionList;
    private UI_HubStateButton exitButton;

    private enum GameObjects
    {
        PromotionLevelBanner,
        UI_NewDishList,
        UI_NewIngredientList,
        UI_NewFunctionList
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    private enum PanelAnimators
    {
        PanelIconImage,
        Background,
        UI_NewIngredientList,
        UI_NewDishList,
        UI_NewFunctionList,
        ExitButton
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));
        Bind<PanelAnimator>(typeof(PanelAnimators));

        rastaurantLevel = GetGameObject((int)GameObjects.PromotionLevelBanner)
            .GetComponent<UI_RastaurantLevel>();
        newDishList = GetGameObject((int)GameObjects.UI_NewDishList)
            .GetComponent<UI_NewDishList>();
        newIngredientList = GetGameObject((int)GameObjects.UI_NewIngredientList)
            .GetComponent<UI_NewIngredientList>();
        newFunctionList = GetGameObject((int)GameObjects.UI_NewFunctionList)
            .GetComponent<UI_NewFunctionList>();

        exitButton = GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton);
        exitButton.Init(Owner);

    }

    protected override IEnumerator OnShow()
    {
        rastaurantLevel.Refresh();
        newDishList.Refresh();
        newIngredientList.Refresh();
        newFunctionList.Refresh();

        GetUI<PanelAnimator>((int)PanelAnimators.PanelIconImage).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.Background).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_NewIngredientList).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_NewDishList).Show();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_NewFunctionList).Show();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.ExitButton).Show();
    }

    protected override IEnumerator OnHide()
    {
        GetUI<PanelAnimator>((int)PanelAnimators.ExitButton).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_NewFunctionList).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_NewDishList).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.UI_NewIngredientList).Hide();
        GetUI<PanelAnimator>((int)PanelAnimators.Background).Hide();
        yield return GetUI<PanelAnimator>((int)PanelAnimators.PanelIconImage).Hide();
    }
}
