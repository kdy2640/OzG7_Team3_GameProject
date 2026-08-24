using System.Collections;
using UnityEngine;

public sealed class UI_RankUpPanel : UI_Base
{
    private UI_RastaurantLevel rastaurantLevel;
    private UI_NewDishList newDishList;
    private UI_NewIngredientList newIngredientList;
    private UI_NewFunctionList newFunctionList;
    private UI_HubStateButton exitButton;

    private PanelAnimator[] panelAnimators;

    private enum GameObjects
    {
        UI_RastaurantLevel,
        UI_NewDishList,
        UI_NewIngredientList,
        UI_NewFunctionList
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

        rastaurantLevel = GetGameObject((int)GameObjects.UI_RastaurantLevel)?
            .GetComponent<UI_RastaurantLevel>();
        newDishList = GetGameObject((int)GameObjects.UI_NewDishList)?
            .GetComponent<UI_NewDishList>();
        newIngredientList = GetGameObject((int)GameObjects.UI_NewIngredientList)?
            .GetComponent<UI_NewIngredientList>();
        newFunctionList = GetGameObject((int)GameObjects.UI_NewFunctionList)?
            .GetComponent<UI_NewFunctionList>();

        exitButton = GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton);
        exitButton?.Init(Owner);

        panelAnimators = GetComponentsInChildren<PanelAnimator>(true);
    }

    protected override IEnumerator OnShow()
    {
        rastaurantLevel?.Refresh();
        newDishList?.Refresh();
        newIngredientList?.Refresh();
        newFunctionList?.Refresh();

        foreach (var animator in panelAnimators)
        {
            if (animator == null) continue;

            if (!animator.PlayOnParentShow) continue;

            if (!animator.gameObject.activeInHierarchy) continue;

            animator.Show();
        }


        yield break;
    }
}
